use crate::{
    config::globals::{self, commands::CREATE_ROOM},
    game::{
        player::{Player, PlayerID},
        room::{Room, RoomId},
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
    rooms: Mutex<HashMap<RoomId, Room>>,
    players: Mutex<HashMap<SocketAddr, Arc<Mutex<Player>>>>,
    next_user_id: AtomicU32,
    next_room_id: AtomicU32,
    active_player_ids: Mutex<HashSet<u32>>,
    active_room_ids: Mutex<HashSet<u32>>,
}

impl ServerContext {
    fn new(server_socket: UdpSocket, broadcast_tx: ChannelSender) -> ServerContext {
        Self {
            next_room_id: AtomicU32::new(1),
            next_user_id: AtomicU32::new(1),
            active_player_ids: Mutex::new(HashSet::new()),
            active_room_ids: Mutex::new(HashSet::new()),
            server_socket,
            broadcast_tx,
            rooms: Mutex::new(HashMap::new()),
            players: Mutex::new(HashMap::new()),
        }
    }

    async fn assign_player_id(&self) -> u32 {
        let mut active_player_ids = self.active_player_ids.lock().await;

        // Take out the current id
        let mut id = self.next_user_id.load(std::sync::atomic::Ordering::SeqCst);

        while active_player_ids.contains(&id) {
            // Cirle back to 1 if hits U32::Max
            id = id.wrapping_add(1);
            if id == 0 {
                id = 1;
            }
        }

        active_player_ids.insert(id);

        // Store the next id for next user
        self.next_user_id
            .store(id.wrapping_add(1), std::sync::atomic::Ordering::SeqCst);
        id
    }

    async fn free_player_id(&self, id: u32) {
        let mut active_player_ids = self.active_player_ids.lock().await;
        println!("UserID: {} is freed", &id);
        active_player_ids.remove(&id);
    }

    async fn assign_room_id(&self) -> u32 {
        let mut active_room_id = self.active_room_ids.lock().await;
        let mut id = self.next_room_id.load(std::sync::atomic::Ordering::SeqCst);

        while active_room_id.contains(&id) {
            id = id.wrapping_add(1);
            if id == 0 {
                id = 1;
            }
        }

        active_room_id.insert(id);
        self.next_room_id
            .store(id.wrapping_add(1), std::sync::atomic::Ordering::SeqCst);

        id
    }

    async fn free_room_id(&self, id: u32) {
        let mut active_room_ids = self.active_room_ids.lock().await;

        println!("RoomID: {} is freed", &id);
        active_room_ids.remove(&id);
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
            // To fix this, the server will have a cleanup method to check inactive
            // user then remove them so the error will no longer happend
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
        Ok(Message::Error(msg)) => {
            println!(
                "Received unexpected error message from client {}: {}",
                &client, msg
            );
        }

        Ok(Message::Ping) => {
            let mut players = context.players.lock().await;
            if let Some(player) = players.get_mut(&client) {
                player.lock().await.last_active = Instant::now();

                println!("Received PONG from {}", client);
            }
        }

        Ok(Message::Handshake(player_name)) => {
            if let Err(e) = accept_client(context.clone(), client, &player_name).await {
                eprintln!(
                    "Failed to accept client {}: {}: {}",
                    client, &player_name, e
                );

                send_error_msg("Handshake message failed ", e, context.clone(), &client).await;
            }
        }

        Ok(Message::Leave(player_id)) => {
            println!("Drop player {}", player_id);
            if let Err(e) = drop_player(context.clone(), client, player_id).await {
                eprintln!("Failed to drop player {} from {}: {}", player_id, client, e);

                send_error_msg("LEAVE message failed", e, context.clone(), &client).await;
            }
        }

        Ok(Message::CreateRoom(room_name, password)) => {
            let players = context.players.lock().await;
            if let Some(player) = players.get(&client) {
                let room_id = context.assign_room_id().await;
                let mut rooms = context.rooms.lock().await;

                // Create a new HashMap with the player
                let mut room_players = HashMap::new();
                room_players.insert(client, player.clone());

                rooms.insert(
                    room_id,
                    Room::new(
                        room_id,
                        room_name.clone(),
                        password.clone(),
                        Mutex::new(room_players),
                    ),
                );

                let mut response = vec![CREATE_ROOM];
                let room_id_bytes = room_id.to_le_bytes();
                response.extend_from_slice(&room_id_bytes);

                if let Err(e) = context.server_socket.send_to(&response, client).await {
                    send_error_msg(
                        "Failed to send CREATE_ROOM response",
                        Box::new(e),
                        context.clone(),
                        &client,
                    )
                    .await;
                } else {
                    println!("Sent CREATE_ROOM message to {}", client);
                }
                println!(
                    "Created room {}: Name={}, Password={}",
                    room_id, room_name, password
                );
            } else {
                let error = Box::new(std::io::Error::new(
                    std::io::ErrorKind::NotFound,
                    "Client not registered",
                )) as Box<dyn Error + Send + Sync>;
                send_error_msg("Create room failed", error, context.clone(), &client).await;
            }
        }

        Ok(Message::JoinRoom(room_id, password)) => {
            let players = context.players.lock().await;

            if let Some(player) = players.get(&client) {
                let rooms = context.rooms.lock().await;

                if let Some(room) = rooms.get(&room_id) {
                    if room.room_pass == password {
                        let mut room_players = room.players.lock().await;

                        room_players.insert(client, player.clone());

                        let mut response = vec![globals::commands::JOIN_ROOM];

                        let room_name_bytes = &room.room_name.as_bytes();

                        let room_name_bytes_len = room_name_bytes.len() as u32;
                        response.extend_from_slice(&room_name_bytes_len.to_le_bytes());
                        response.extend_from_slice(room_name_bytes);

                        if let Err(e) = context.server_socket.send_to(&response, client).await {
                            eprintln!(
                                "Cannot add player {} to room {}: {}",
                                player.lock().await.id,
                                room.id,
                                e
                            );
                            send_error_msg(
                                "Failed to send JOIN_ROOM response",
                                Box::new(e),
                                context.clone(),
                                &client,
                            )
                            .await;
                        } else {
                            println!("Player {} joined room {}", player.lock().await.id, room.id);
                        }
                    } else {
                        let error = Box::new(std::io::Error::new(
                            std::io::ErrorKind::InvalidInput,
                            "Incorrect password",
                        )) as Box<dyn Error + Send + Sync>;
                        send_error_msg("Join room failed", error, context.clone(), &client).await;
                    }
                } else {
                    let error = Box::new(std::io::Error::new(
                        std::io::ErrorKind::NotFound,
                        "Room not existed",
                    )) as Box<dyn Error + Send + Sync>;
                    send_error_msg("Join room failed", error, context.clone(), &client).await;
                }
            }
        }

        Err(e) => {
            println!("Something went wrong: {:?}", e);
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

/////////////////////////////////////////////////////

// Send message error to client
async fn send_error_msg(
    msg: &str,
    e: Box<dyn Error + Send + Sync>,
    context: Arc<ServerContext>,
    client: &SocketAddr,
) {
    let error_msg = Message::Error(format!("{msg}: {e}"));

    match context
        .server_socket
        .send_to(&error_msg.serialize(), client)
        .await
    {
        Ok(_) => {
            println!("Send error message to client");
        }

        Err(e) => {
            eprintln!("Can not send error message to player: {e}")
        }
    }
}

// Accept new player
async fn accept_client(
    context: Arc<ServerContext>,
    client: SocketAddr,
    player_name: &str,
) -> Result<(), Box<dyn Error + Send + Sync>> {
    let mut players = context.players.lock().await;
    let ack_msg: Vec<u8>;

    if let Some(existing_player) = players.get(&client) {
        ack_msg = Message::Ack(existing_player.lock().await.id).serialize();
    } else {
        let player_id = context.assign_player_id().await;

        let new_player = Arc::new(Mutex::new(Player::new(player_id)));

        println!(
            "Player {}: {player_name} joined the server",
            new_player.lock().await.id
        );

        players.insert(client, new_player);
        ack_msg = Message::Ack(player_id).serialize();
    }

    println!("Sending Ack to {}", client);
    context.server_socket.send_to(&ack_msg, client).await?;

    let sent_message = Message::deserialize(&ack_msg).unwrap();
    message::trace(format!("Sent: {:?}", sent_message));

    Ok(())
}

// Remove player
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
        context.free_player_id(player.lock().await.id).await;
    }

    Ok(())
}

/// Send ping to healthcheck
async fn ping_sender(context: Arc<ServerContext>) {
    let mut interval = tokio::time::interval(globals::PING_INTERVAL_MS);

    // Sending ping to healthcheck server every 20s
    loop {
        // println!("SENT PING");
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
            if Instant::now().duration_since(player.lock().await.last_active) > inactivity_timeout {
                println!(
                    "Removing inactive client: {} (ID: {})",
                    addr,
                    player.lock().await.id
                );

                to_remove.push(*addr);
            }
        }

        for addr in to_remove {
            if let Some(player) = players.remove(&addr) {
                context.free_player_id(player.lock().await.id).await;
            }
        }
    }
}
