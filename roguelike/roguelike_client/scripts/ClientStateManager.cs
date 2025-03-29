using System.Collections.Generic;
using Godot;

public partial class ClientStateManager : Node
{
    public static ClientStateManager Instance { get; private set; }
    public Player LocalPlayer { get; private set; }
    public Room CurrentRoom { get; private set; }

    private int _lastProcessedSequence = -1;

    public override void _Ready()
    {
        Instance = this;
        GD.Print("ClientStateManagemer init!");
    }

    public void UpdateConnectionStatus(bool connected, int playerId, string username)
    {
        if (connected)
        {
            LocalPlayer = new Player(playerId, username);
            GD.Print($"LocalPlayer Created: {username} (ID: {playerId})");
        }
        else
        {
            LocalPlayer = null;
            CurrentRoom = null;
            _lastProcessedSequence = -1;
            GD.Print("Disconnected, clear state");
        }
    }

    public void AddPlayerToCurrentRoom(int playerId, string username)
    {
        if (CurrentRoom != null && !CurrentRoom.Players.ContainsKey(playerId))
        {
            var player = new Player(playerId, username);
            CurrentRoom.AddPlayer(player);
            GD.Print($"Added player {username} (ID: {playerId}) to room {CurrentRoom.RommId}");
        }
    }

    public void SetCurrentRoom(int roomId)
    {
        CurrentRoom.RommId = roomId;
    }

    public void RemovePlayerFromCurrentRoom(int playerId)
    {
        if (CurrentRoom != null)
        {
            CurrentRoom.RemovePlayer(playerId);
            GD.Print($"Removed player ID: {playerId} from room {CurrentRoom.RommId}");
        }
    }
}
