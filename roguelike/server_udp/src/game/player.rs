use cgmath::Vector2;

pub type PlayerID = u32;

#[derive(Clone, Copy, Debug, PartialEq)]
pub struct Player {
    pub id: PlayerID,
    pub pos: Vector2<f32>,
    pub velocity: Vector2<f32>,
}

impl Default for Player {
    fn default() -> Self {
        Self {
            id: 0,
            pos: Vector2::new(0.0, 0.0),
            velocity: Vector2::new(0.0, 0.0),
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
