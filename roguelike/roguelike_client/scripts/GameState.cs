using System.Collections.Generic;
using Godot;

public class RoomState
{
    public int RoomId { get; set; } = -1;
    public string RoomName { get; set; } = "";
    public Dictionary<int, PlayerState> Players { get; set; } = new Dictionary<int, PlayerState>();
    public Dictionary<int, EnemyState> Enemies { get; set; } = new Dictionary<int, EnemyState>();
}

public class PlayerState
{
    public int PlayerId { get; set; }
    public Vector2 Position { get; set; }
    public Vector2 Velocity { get; set; }
    public int Health { get; set; } = 100;
    public bool Connected { get; set; } = true;
}

public class EnemyState
{
    public int EnemyId { get; set; }
    public Vector2 Position { get; set; }
    public int Health { get; set; } = 50;
}
