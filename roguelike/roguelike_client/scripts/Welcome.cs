using System;
using Godot;

public partial class Welcome : Control
{
    private LineEdit _nameInput;
    private Button _connectBtn;
    private Label _status;
    private Button _disconnect;
    private Button _goToLobby;
    private NetworkManagement _networkManagement;

    public override void _Ready()
    {
        _nameInput = GetNode<LineEdit>("LineEdit");
        _connectBtn = GetNode<Button>("Button");
        _status = GetNode<Label>("Status");
        _disconnect = GetNode<Button>("Disconnect");
        _networkManagement = GetNode<NetworkManagement>("%NetworkManagement");

        _goToLobby = GetNode<Button>("GoToLobby");
        _goToLobby.Hide();

        _connectBtn.Pressed += OnConnectedButtonPressed;
        _disconnect.Pressed += OnDisconnectedButtonPressed;

        _networkManagement.ConnectionStatusChanged += OnConnectionStatusChanged;

        UpdateUI(false, -1);
    }

    private void OnConnectedButtonPressed()
    {
        string username = _nameInput.Text.Trim();
        if (string.IsNullOrEmpty(username))
        {
            _status.Text = "Status: Require username before entering!";
        }
        else
        {
            _status.Text = "Status: Connecting...";
            _networkManagement.ConnectToServer(username);

        }
    }

    private void OnDisconnectedButtonPressed()
    {
        GD.Print("Disconnect server....");
        _status.Text = "Status: Disconnecting...";
        _networkManagement.DisconnectToServer();
        UpdateUI(_networkManagement.Connected, _networkManagement.PlayerId);
    }

    private void OnGoToLobbyPressed()
    {
        GD.Print("Going to lobby...");
        /*GetTree().ChangeSceneToFile("res://Scenes/Lobby.tscn"); // Adjust path*/
    }

    private void OnConnectionStatusChanged(bool connected, int playerId)
    {
        GD.Print($"Connection changed: Connected={connected}, PlayerId={playerId}");
        UpdateUI(connected, playerId);
    }

    private void UpdateUI(bool connected, int playerId)
    {
        if (connected)
        {
            _status.Text = $"Status: Connected (Player ID: {playerId})";

            _disconnect.Show();
            _disconnect.MouseDefaultCursorShape = CursorShape.PointingHand;

            _connectBtn.Hide();

            _goToLobby.Show();
            _goToLobby.Pressed += OnGoToLobbyPressed;
            _goToLobby.MouseDefaultCursorShape = CursorShape.PointingHand;
        }
        else
        {
            _status.Text = "Status: Not Connected";
            _disconnect.Hide();

            _connectBtn.Show();
            _connectBtn.Text = "Connect";
            _connectBtn.MouseDefaultCursorShape = CursorShape.PointingHand;

            _goToLobby.Hide();
        }
    }
}
