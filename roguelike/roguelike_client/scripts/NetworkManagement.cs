using System;
using System.Text;
using Godot;

public partial class NetworkManagement : Node
{
    [Signal]
    public delegate void ConnectionStatusChangedEventHandler(bool connected, int playerId);

    private NetworkClient _networkClient;
    private string _pendingUsername = "";

    public static NetworkManagement Instance { get; private set; }

    public override void _Ready()
    {
        // Connect to server
        _networkClient = new NetworkClient();
        AddChild(_networkClient);

        // Set the singleton instance
        Instance = this;
        GD.Print("Network manager initialized");
    }

    public void UpdateConnectionStatus(bool connected, int playerId)
    {
        ClientStateManager.Instance.UpdateConnectionStatus(
            connected,
            playerId,
            connected ? _pendingUsername : "unknown name"
        );
        GD.Print($"Emitting ConnectionStatusChanged: Connected={connected}, PlayerId={playerId}");
        EmitSignal(SignalName.ConnectionStatusChanged, connected, playerId);
    }

    public void ConnectToServer(string username)
    {
        _pendingUsername = username;
        _networkClient.ConnectToServer(username);
    }

    public void DisconnectToServer()
    {
        _networkClient.DisconnectServer();
    }

    public override void _Process(double delta) { }
}
