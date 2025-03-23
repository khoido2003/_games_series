using System;
using Godot;

public partial class Welcome : Control
{
    private LineEdit _nameInput;
    private Button _connectBtn;
    private Label _status;
    private NetworkManagement networkManagement;

    public override void _Ready()
    {
        _nameInput = GetNode<LineEdit>("LineEdit");
        _connectBtn = GetNode<Button>("Button");
        _status = GetNode<Label>("Status");

        _connectBtn.Pressed += OnConnectedButtonPressed;

        networkManagement = GetNode<NetworkManagement>("%NetworkManagement");
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

            networkManagement.ConnectToServer(username);
        }
    }
}
