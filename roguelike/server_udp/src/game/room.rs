use std::{collections::HashMap, net::SocketAddr, sync::Arc};
use tokio::sync::Mutex;

use super::player::Player;

pub type RoomId = u32;
pub type RoomName = String;
pub type RoomPass = String;

#[derive(Debug)]
pub struct Room {
    pub id: RoomId,
    pub room_name: RoomName,
    pub room_pass: RoomPass,
    pub players: Mutex<HashMap<SocketAddr, Arc<Mutex<Player>>>>,
}

impl Room {
    pub fn new(
        id: RoomId,
        room_name: RoomName,
        room_pass: RoomPass,
        players: Mutex<HashMap<SocketAddr, Arc<Mutex<Player>>>>,
    ) -> Self {
        Room {
            id,
            room_name,
            room_pass,
            players,
        }
    }
}
