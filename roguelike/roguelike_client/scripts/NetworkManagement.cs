using System;
using System.Text;
using Godot;

public partial class NetworkManagement : Node
{
    [Signal]
    public delegate void ConnectionStatusChangedEventHandler(bool connected, int playerId);

    private NetworkClient _networkClient;

    public string Username { get; private set; } = "";
    public bool Connected { get; private set; } = false;
    public int PlayerId { get; private set; } = -1;

    public override void _Ready()
    {
        // Connect to server
        _networkClient = new NetworkClient();
        AddChild(_networkClient);

        GD.Print("Network manager initialized");
    }

    public void UpdateConnectionStatus(bool connected, int playerId)
    {
        Connected = connected;
        PlayerId = playerId;
        GD.Print($"Emitting ConnectionStatusChanged: Connected={Connected}, PlayerId={PlayerId}");
        EmitSignal(SignalName.ConnectionStatusChanged, Connected, PlayerId);
    }

    public void ConnectToServer(string username)
    {
        Username = username;
        _networkClient.ConnectToServer(username);
    }

    public void DisconnectToServer()
    {
        Connected = false;
        _networkClient.DisconnectServer();
    }

    public override void _Process(double delta) { }
}
