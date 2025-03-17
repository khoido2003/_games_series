use std::{io, sync::atomic::AtomicBool};

use cgmath::num_traits::ToBytes;

use crate::{
    config::globals::commands::{ACK, CREATE_ROOM, HANDSHAKE, LEAVE, PING, PLAYER_INPUT},
    game::{
        player::{self, PlayerID},
        room::{Room, RoomId},
    },
};

#[derive(Debug, PartialEq)]
pub enum InputAction {
    Move(f32, f32),
    Shoot(f32, f32),
}

#[derive(Debug, PartialEq)]
pub enum Message {
    /// Client/Server healthcheck
    Ping,

    /// Handshake on connect
    Handshake,

    /// Server acknowledge handshake with PlayerId
    Ack(PlayerID),

    /// Create new room/match
    CreateRoom,

    /// Leaves room/match
    Leave(PlayerID),

    /// Join room/match
    JoinRoom(RoomId),
    ///// Client sends input
    //PlayerInput(PlayerID, InputAction),
    //
    ///// Server sends full room state
    //RoomSnapshot(Room),
}

impl Message {
    pub fn serialize(&self) -> Vec<u8> {
        match self {
            Message::Ping => vec![PING],
            Message::Handshake => vec![HANDSHAKE],
            Message::Ack(player_id) => {
                let mut packet = vec![ACK];
                packet.extend_from_slice(&player_id.to_le_bytes());
                packet
            }
            Message::Leave(player_id) => {
                let mut packet = vec![LEAVE];
                packet.extend_from_slice(&player_id.to_le_bytes());
                packet
            }

            Message::CreateRoom => vec![CREATE_ROOM],
            Message::JoinRoom(room_id) => {
                let mut packet = vec![PLAYER_INPUT];
                packet.extend_from_slice(&room_id.to_le_bytes());
                packet
            }
        }
    }

    pub fn deserialize(packet: &[u8]) -> Result<Message, io::Error> {
        if packet.is_empty() {
            return Err(io::Error::new(io::ErrorKind::InvalidData, "Empty packet"));
        }

        match packet[0] {
            PING => Ok(Message::Ping),
            HANDSHAKE => Ok(Message::Handshake),

            ACK => {
                let player_id = u32::from_le_bytes([packet[1], packet[2], packet[3], packet[4]]);
                Ok(Message::Ack(player_id))
            }

            LEAVE if packet.len() >= 5 => {
                let player_id = u32::from_le_bytes([packet[1], packet[2], packet[3], packet[4]]);

                Ok(Message::Leave(player_id))
            }

            _ => Err(io::Error::new(
                io::ErrorKind::InvalidData,
                format!(
                    "Unknow command {} or insufficient length {}",
                    packet[0],
                    packet.len()
                ),
            )),
        }
    }
}

////////////////////////////////////////////////

static TRACE_ENABLED: AtomicBool = AtomicBool::new(false);

pub fn set_trace(enabled: bool ) {
    TRACE_ENABLED.store(enabled, std::sync::atomic::Ordering::Relaxed);
}

pub fn trace(s: String) {
    if TRACE_ENABLED.load(std::sync::atomic::Ordering::Relaxed) {
        println!("[TRACE] {s}");
    }
}
