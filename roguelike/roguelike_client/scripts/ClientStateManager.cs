using System.Collections.Generic;
using Godot;

public partial class ClientStateManager : Node
{
    public static ClientStateManager Instance { get; private set; }
    public Player LocalPlayer { get; private set; }
    public Room CurrentRoom { get; private set; }
    public Dictionary<int, Room> AvailableRooms { get; private set; } = new Dictionary<int, Room>();

    private List<InputAction> _inputHistory = new List<InputAction>();
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
            AvailableRooms.Clear();
            _inputHistory.Clear();
            _lastProcessedSequence = -1;
            GD.Print("Disconnected, clear state");
        }
    }

    public void SetCurrentRoom(int roomId)
    {
        CurrentRoom.RommId = roomId;
    }
}
