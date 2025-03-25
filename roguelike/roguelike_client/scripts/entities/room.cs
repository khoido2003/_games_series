using System.Collections.Generic;
using Godot;

public class Room
{
    public int RommId { get; set; } = -1;
    public string RoomName { get; set; } = "";
    public Dictionary<int, Player> Players { get; set; } = new Dictionary<int, Player>();

    public Room(int roomId, string roomName)
    {
        RoomName = roomName;
        RommId = roomId;
    }

    public void AddPlayer(Player user)
    {
        Players[user.PlayerId] = user;
    }

    public void RemovePlayer(int playerId)
    {
        Players.Remove(playerId);
    }
}
