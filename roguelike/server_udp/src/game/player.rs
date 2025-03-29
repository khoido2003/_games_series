use std::time::Instant;

use super::Position;

pub type PlayerID = u32;
pub type PlayerName = String;

#[derive(Debug)]
pub struct Player {
    pub player_name: PlayerName,
    pub id: PlayerID,
    pub position: Position,
    pub velocity: Position,
    pub health: i32,
    pub last_active: Instant,
}

impl Default for Player {
    fn default() -> Self {
        Self {
            player_name: String::new(),
            id: 0,
            position: Position { x: 0.0, y: 0.0 },
            velocity: Position { x: 0.0, y: 0.0 },
            health: 100,
            last_active: Instant::now(),
        }
    }
}

impl Player {
    pub fn new(id: PlayerID) -> Self {
        let mut player = Player::default();
        player.id = id;
        player
    }
}
