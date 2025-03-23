pub mod commands {
    pub const PING: u8 = 0;
    pub const HANDSHAKE: u8 = 1;
    pub const ACK: u8 = 2;
    pub const LEAVE: u8 = 3;
    pub const REPLICATE: u8 = 4;
    pub const CREATE_ROOM: u8 = 5;
    pub const JOIN_ROOM: u8 = 6;
    pub const PLAYER_INPUT: u8 = 7;
    pub const ROOM_SNAPSHOT: u8 = 8;
}

pub const DEFAULT_PORT: u16 = 5678;
pub const CONNECTION_TIMEOUT_SEC: std::time::Duration = std::time::Duration::from_secs(5);
pub const PING_INTERVAL_MS: std::time::Duration = std::time::Duration::from_secs(15);
