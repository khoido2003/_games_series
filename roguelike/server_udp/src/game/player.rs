use std::time::Instant;

use cgmath::Vector2;

pub type PlayerID = u32;
pub type PlayerName = String;

#[derive(Clone, Debug, PartialEq)]
pub struct Player {
    pub player_name: PlayerName,
    pub id: PlayerID,
    pub pos: Vector2<f32>,
    pub velocity: Vector2<f32>,
    pub last_active: Instant,
}

impl Default for Player {
    fn default() -> Self {
        Self {
            player_name: String::new(),
            id: 0,
            pos: Vector2::new(0.0, 0.0),
            velocity: Vector2::new(0.0, 0.0),
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
