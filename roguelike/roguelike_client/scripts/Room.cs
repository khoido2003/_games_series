using System.Collections.Generic;
using Godot;

public partial class Room : Node2D
{
    public int RommId { get; set; } = -1;
    public string RoomName { get; set; } = "";
    public Dictionary<int, Player> Players { get; set; } = new Dictionary<int, Player>();

    public Room(int roomId, string roomName)
    {
        RoomName = roomName;
        RommId = roomId;
    }

    public Room()
    {
        // Default initialization or leave empty
    }

    public void AddPlayer(Player user)
    {
        Players[user.PlayerId] = user;
    }

    public void RemovePlayer(int playerId)
    {
        Players.Remove(playerId);
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() { }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) { }
}
