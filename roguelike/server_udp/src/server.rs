use crate::{
    config::globals::{self},
    game::{
        player::{Player, PlayerID},
        room::Room,
    },
    network::message::{self, Message},
};
use std::{
    collections::{HashMap, HashSet},
    error::Error,
    net::SocketAddr,
    sync::{Arc, atomic::AtomicU32},
    time::{Duration, Instant},
};
use tokio::{
    self,
    net::UdpSocket,
    sync::{Mutex, mpsc},
    time::interval,
};

//////////////////////////////////////////////////////////////////

type ServerSessionResult = Result<(), Box<dyn Error + Send + Sync>>;

type ChannelSender = mpsc::UnboundedSender<BroadcastMessage>;
type ChannelReceiver = mpsc::UnboundedReceiver<BroadcastMessage>;

struct BroadcastMessage {
    msg: Vec<u8>,
    excluded_client: Option<SocketAddr>,
}

struct ServerContext {
    server_socket: UdpSocket,
    broadcast_tx: ChannelSender,
    rooms: HashMap<u32, Room>,
    players: Mutex<HashMap<SocketAddr, Player>>,
    next_id: AtomicU32,
    active_ids: Mutex<HashSet<u32>>,
}

impl ServerContext {
    fn new(server_socket: UdpSocket, broadcast_tx: ChannelSender) -> ServerContext {
        Self {
            next_id: AtomicU32::new(1),
            active_ids: Mutex::new(HashSet::new()),
            server_socket,
            broadcast_tx,
            rooms: HashMap::new(),
            players: Mutex::new(HashMap::new()),
        }
    }

    async fn assign_player_id(&self) -> u32 {
        let mut active_ids = self.active_ids.lock().await;

        // Take out the current id
        let mut id = self.next_id.load(std::sync::atomic::Ordering::SeqCst);

        while active_ids.contains(&id) {
            // Cirle back to 1 if hits U32::Max
            id = id.wrapping_add(1);
            if id == 0 {
                id = 1;
            }
        }

        active_ids.insert(id);

        // Store the next id for next user
        self.next_id
            .store(id.wrapping_add(1), std::sync::atomic::Ordering::SeqCst);
        id
    }

    async fn free_player_id(&self, id: u32) {
        let mut active_ids = self.active_ids.lock().await;
        println!("ID: {} is freed", &id);
        active_ids.remove(&id);
    }
}

//-------------------------------------

// Function to create new server
pub async fn start_server(port: u16) -> ServerSessionResult {
    match tokio::time::timeout(globals::CONNECTION_TIMEOUT_SEC, async {
        // Use 0.0.0.0 to allow listen from anywhere
        let address = format!("0.0.0.0:{port}");
        let server_socket = UdpSocket::bind(&address).await?;
        let (broadcast_tx, broadcast_rx) = mpsc::unbounded_channel::<BroadcastMessage>();

        let context = Arc::new(ServerContext::new(server_socket, broadcast_tx));

        tokio::spawn(listen_handler(context.clone()));
        tokio::spawn(broadcast_handler(context.clone(), broadcast_rx));

        // Healthcheck server
        tokio::spawn(ping_sender(context.clone()));

        // Cleanup inactive player
        tokio::spawn(cleanup_inactive(context.clone()));

        Ok(()) as ServerSessionResult
    })
    .await
    {
        Ok(_) => Ok(()),
        Err(e) => Err(format!(
            "Server took too long to start - timeout after {} seconds: {e}",
            globals::CONNECTION_TIMEOUT_SEC.as_secs()
        )
        .into()),
    }
}

async fn broadcast_handler(context: Arc<ServerContext>, mut broadcast_rx: ChannelReceiver) {
    while let Some(message) = broadcast_rx.recv().await {
        let players = context.players.lock().await;

        for (addr, _) in players.iter() {
            if message.excluded_client != Some(*addr) {
                if let Err(e) = context.server_socket.send_to(&message.msg, addr).await {
                    eprintln!("Error sending broadcast to {}: {}", addr, e);
                }
            }
        }
    }
}

// Handle request come in
async fn listen_handler(context: Arc<ServerContext>) {
    loop {
        let mut buf = vec![0u8; 1024];

        match context.server_socket.recv_from(&mut buf).await {
            Ok((len, client)) => {
                if len > 0 {
                    let request_msg = String::from_utf8_lossy(&buf[..len]);
                    println!("{}", request_msg);

                    // Handle in binary form
                    let packet = buf[..len].to_vec();

                    tokio::spawn(process_client_message(context.clone(), client, packet));
                }
            }

            // This error happend when client close connection but server keep sending
            // ping to that client and client machine send back the error
            Err(e) if e.raw_os_error() == Some(10054) => {
                println!("client disconnected (os error 10054), continue...")
            }
            Err(e) => {
                eprint!("Error receiving UDP packet: {e}");
            }
        }
    }
}

async fn process_client_message(context: Arc<ServerContext>, client: SocketAddr, packet: Vec<u8>) {
    if packet.is_empty() {
        return;
    }
    let command = packet[0];
    println!("Command received: {} (0x{:02x})", command, command);

    match Message::deserialize(&packet) {
        Ok(Message::Ping) => {
            let mut players = context.players.lock().await;
            if let Some(player) = players.get_mut(&client) {
                player.last_active = Instant::now();

                println!("Received PONG from {}", client);
            }
        }

        Ok(Message::Handshake(player_name)) => {
            if let Err(e) = accept_client(context.clone(), client, &player_name).await {
                eprintln!(
                    "Failed to accept client {}: {}: {}",
                    client, &player_name, e
                );
            }
        }
        Ok(Message::Leave(player_id)) => {
            if let Err(e) = drop_player(context.clone(), client, player_id).await {
                eprintln!("Failed to drop player {} from {}: {}", player_id, client, e);
            }
        }

        _ => {
            println!("Not a command {}", String::from_utf8_lossy(&packet));

            // Send the message back to the client to inform wrong format
            let mes = format!(
                "Not a command: {}\npacket: {:?}",
                String::from_utf8_lossy(&packet),
                &packet
            );

            if let Err(e) = context.server_socket.send_to(mes.as_bytes(), client).await {
                eprintln!("Can not send back the message to client {}\n {}", client, e);
            }
        }
    }
}

async fn accept_client(
    context: Arc<ServerContext>,
    client: SocketAddr,
    player_name: &str,
) -> Result<(), Box<dyn Error + Send + Sync>> {
    let mut players = context.players.lock().await;
    let ack_msg: Vec<u8>;

    if let Some(existing_player) = players.get(&client) {
        ack_msg = Message::Ack(existing_player.id).serialize();
    } else {
        let player_id = context.assign_player_id().await;
        let new_player = Player::new(player_id);
        println!("Player {}: {player_name} joined the server", &new_player.id);

        players.insert(client, new_player);
        ack_msg = Message::Ack(player_id).serialize();
    }

    println!("Sending Ack to {}", client);
    context.server_socket.send_to(&ack_msg, client).await?;

    let sent_message = Message::deserialize(&ack_msg).unwrap();
    message::trace(format!("Sent: {:?}", sent_message));
    Ok(())
}

async fn drop_player(
    context: Arc<ServerContext>,
    client: SocketAddr,
    player_id: PlayerID,
) -> Result<(), Box<dyn Error + Sync + Send>> {
    let mut players = context.players.lock().await;

    if let Some(player) = players.remove(&client) {
        println!("Player {player_id} left the server");

        context.broadcast_tx.send(BroadcastMessage {
            msg: Message::Leave(player_id).serialize(),
            excluded_client: Some(client),
        })?;
        context.free_player_id(player.id).await;
    }

    Ok(())
}

/// Send ping to healthcheck
async fn ping_sender(context: Arc<ServerContext>) {
    let mut interval = tokio::time::interval(globals::PING_INTERVAL_MS);

    // Sending ping to healthcheck server every 20s
    loop {
        println!("SENT PING");
        interval.tick().await;
        let _ = context.broadcast_tx.send(BroadcastMessage {
            msg: Message::Ping.serialize(),
            excluded_client: None,
        });
    }
}

/// Cleanup inactive player after 30s
async fn cleanup_inactive(context: Arc<ServerContext>) {
    let mut interval = tokio::time::interval(Duration::from_secs(10));

    let inactivity_timeout = Duration::from_secs(30);

    loop {
        interval.tick().await;

        let mut players = context.players.lock().await;
        let mut to_remove = Vec::new();

        for (addr, player) in players.iter() {
            if Instant::now().duration_since(player.last_active) > inactivity_timeout {
                println!("Removing inactive client: {} (ID: {})", addr, player.id);

                to_remove.push(*addr);
            }
        }

        for addr in to_remove {
            if let Some(player) = players.remove(&addr) {
                context.free_player_id(player.id).await;
            }
        }
    }
}
