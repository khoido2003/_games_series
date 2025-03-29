use super::Position;

#[derive(Debug)]
pub struct Enemy {
    pub id: u32,
    pub position: Position,
    pub health: i32,
    pub speed: f32,
}
