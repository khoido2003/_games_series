use std::{io, sync::atomic::AtomicBool, u16, usize};

use cgmath::num_traits::ToBytes;

use crate::{
    config::globals::commands::{
        ACK, CREATE_ROOM, HANDSHAKE, JOIN_ROOM, LEAVE, PING, PLAYER_INPUT,
    },
    game::{
        player::{PlayerID, PlayerName},
        room::{RoomId, RoomName, RoomPass},
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
    Handshake(PlayerName),

    /// Server acknowledge handshake with PlayerId
    Ack(PlayerID),

    /// Create new room/match
    CreateRoom(RoomName, RoomPass),

    /// Leaves room/match
    Leave(PlayerID),

    /// Join room/match
    JoinRoom(RoomId, RoomPass),
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
            Message::Handshake(player_name) => {
                let mut packet = vec![HANDSHAKE];
                let name_bytes = player_name.as_bytes();
                let length = name_bytes.len() as u16;

                // Add name length
                packet.extend_from_slice(&length.to_le_bytes());

                // Add name
                packet.extend_from_slice(name_bytes);

                packet
            }

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

            Message::CreateRoom(room_name, password) => {
                let mut packet = vec![CREATE_ROOM];
                let name_bytes = room_name.as_bytes();
                let pass_bytes = password.as_bytes();

                packet.extend_from_slice(&(name_bytes.len() as u16).to_le_bytes());
                packet.extend_from_slice(name_bytes);
                packet.extend_from_slice(&(pass_bytes.len()).to_le_bytes());
                packet.extend_from_slice(pass_bytes);

                packet
            }
            Message::JoinRoom(room_id, password) => {
                let mut packet = vec![JOIN_ROOM];
                packet.extend_from_slice(&room_id.to_le_bytes());
                let pass_bytes = password.as_bytes();
                packet.extend_from_slice(&(pass_bytes.len() as u16).to_le_bytes());
                packet.extend_from_slice(pass_bytes);
                packet
            }
            _ => {
                vec![]
            }
        }
    }

    pub fn deserialize(packet: &[u8]) -> Result<Message, io::Error> {
        if packet.is_empty() {
            return Err(io::Error::new(io::ErrorKind::InvalidData, "Empty packet"));
        }

        println!("Deserializing packet: {:?}", packet);
        match packet[0] {
            PING => Ok(Message::Ping),

            HANDSHAKE if packet.len() >= 3 => {
                // Read the length of the username string
                let length = u16::from_le_bytes([packet[1], packet[2]]) as usize;

                if packet.len() < 3 + length {
                    return Err(io::Error::new(
                        io::ErrorKind::InvalidData,
                        "Packet too short! Missing name",
                    ));
                }

                let name_bytes = &packet[3..3 + length];
                let player_name = String::from_utf8(name_bytes.to_vec())
                    .map_err(|e| io::Error::new(io::ErrorKind::InvalidData, e))?;

                Ok(Message::Handshake(player_name))
            }

            ACK => {
                let player_id = u32::from_le_bytes([packet[1], packet[2], packet[3], packet[4]]);
                Ok(Message::Ack(player_id))
            }

            LEAVE if packet.len() >= 5 => {
                let player_id = u32::from_le_bytes([packet[1], packet[2], packet[3], packet[4]]);

                Ok(Message::Leave(player_id))
            }

            CREATE_ROOM if packet.len() > 5 => {
                let room_name_len = u16::from_le_bytes([packet[1], packet[2]]) as usize;
                if packet.len() < 3 + room_name_len + 2 {
                    return Err(io::Error::new(
                        io::ErrorKind::InvalidData,
                        "Packet too short",
                    ));
                }

                let room_name = String::from_utf8(packet[3..3 + room_name_len].to_vec())
                    .map_err(|e| io::Error::new(io::ErrorKind::InvalidData, e))?;

                let pass_offset = 3 + room_name_len;
                let pass_len =
                    u16::from_le_bytes([packet[pass_offset], packet[pass_offset + 1]]) as usize;
                let pass =
                    String::from_utf8(packet[pass_offset + 2..pass_offset + pass_len + 2].to_vec())
                        .map_err(|e| io::Error::new(io::ErrorKind::InvalidData, e))?;

                Ok(Message::CreateRoom(room_name, pass))
            }

            JOIN_ROOM if packet.len() >= 7 => {
                let room_id = u32::from_le_bytes(packet[1..5].try_into().map_err(|_| {
                    io::Error::new(io::ErrorKind::InvalidData, "Packet must be 4 bytes")
                })?);

                let pass_len = u16::from_le_bytes([packet[5], packet[6]]) as usize;
                if packet.len() < 7 + pass_len {
                    return Err(io::Error::new(
                        io::ErrorKind::InvalidData,
                        "Packet too short for JOIN_ROOM",
                    ));
                }

                let password = String::from_utf8(packet[7..7 + pass_len].to_vec())
                    .map_err(|e| io::Error::new(io::ErrorKind::InvalidData, e))?;

                Ok(Message::JoinRoom(room_id, password))
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

pub fn set_trace(enabled: bool) {
    TRACE_ENABLED.store(enabled, std::sync::atomic::Ordering::Relaxed);
}

pub fn trace(s: String) {
    if TRACE_ENABLED.load(std::sync::atomic::Ordering::Relaxed) {
        println!("[TRACE] {s}");
    }
}
