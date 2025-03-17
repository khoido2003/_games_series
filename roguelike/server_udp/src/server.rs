use crate::{
    config::globals::{
        self,
        commands::{CREATE_ROOM, HANDSHAKE},
    },
    game::{player::Player, room::Room},
    network::message::{self, Message},
};
use std::{
    collections::HashMap,
    error::Error,
    net::SocketAddr,
    sync::{Arc, atomic::AtomicU32},
};
use tokio::{
    self,
    net::UdpSocket,
    sync::{Mutex, mpsc},
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
    player_id_counter: AtomicU32,
    server_socket: UdpSocket,
    broadcast_tx: ChannelSender,
    rooms: HashMap<u32, Room>,
    players: Mutex<HashMap<SocketAddr, Player>>,
}

impl ServerContext {
    fn new(server_socket: UdpSocket, broadcast_tx: ChannelSender) -> ServerContext {
        Self {
            player_id_counter: AtomicU32::new(1),
            server_socket,
            broadcast_tx,
            rooms: HashMap::new(),
            players: Mutex::new(HashMap::new()),
        }
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
        Ok(Message::Handshake) => {}

        Ok(Message::Leave(player_id)) => {}

        _ => (),
    }
}

async fn accept_client(
    context: Arc<ServerContext>,
    client: SocketAddr,
) -> Result<(), Box<dyn Error + Send + Sync>> {
    let mut players = context.players.lock().await;
    let ack_msg: Vec<u8>;

    if let Some(existing_player) = players.get(&client) {
        ack_msg = Message::Ack(existing_player.id).serialize();
    } else {
        let new_player = Player::new(
            context
                .player_id_counter
                .fetch_add(1, std::sync::atomic::Ordering::SeqCst),
        );

        players.insert(client, new_player);
        println!("Player {} joined the server", new_player.id);
        ack_msg = Message::Ack(new_player.id).serialize();
    }

    context.server_socket.send_to(&ack_msg, client).await?;
    
    message::trace(format!("Sent: {}", String::from_utf8_lossy(&ack_msg)));
    Ok(())
}
