using System;
using System.Text;
using Godot;

public partial class NetworkManagement : Node
{
    public string Username { get; private set; } = "";
    private NetworkClient _networkClient;
    public bool Connected => _networkClient.Connected;

    public override void _Ready()
    {
        // Connect to server
        _networkClient = new NetworkClient();
        AddChild(_networkClient);

        GD.Print("Network manager initialized");
    }

    public void ConnectToServer(string username)
    {
        Username = username;
        _networkClient.ConnectToServer(username);
    }

    public override void _Process(double delta) { }
}
