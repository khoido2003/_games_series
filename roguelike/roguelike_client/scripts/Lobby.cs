using System;
using Godot;

public partial class Lobby : Control
{
    private Button _createRoomBtn;
    private Button _joinRoomBtn;
    private Label _idLabel;
    private Label _usernameLabel;
    private Label _status;
    private Window _createRoomPopUp;
    private Window _joinRoomPopUp;

    private LineEdit _createRoomNameInput;
    private LineEdit _createPasswordInput;
    private Button _createConfirmBtn;
    private Label _createStatus;

    private LineEdit _joinRoomIdInput;
    private LineEdit _joinPasswordInput;
    private Button _joinConfirmBtn;
    private Label _joinStatus;

    public override void _Ready()
    {
        // Main UI elements
        _createRoomBtn = GetNode<Button>("CreateBtn");
        _joinRoomBtn = GetNode<Button>("JoinBtn");
        _usernameLabel = GetNode<Label>("Username");
        _idLabel = GetNode<Label>("Id");
        _status = GetNode<Label>("Label");
        _createRoomPopUp = GetNode<Window>("CreateRoom");
        _joinRoomPopUp = GetNode<Window>("JoinRoom");

        _createRoomPopUp.Hide();
        _joinRoomPopUp.Hide();

        // Create popup elements
        _createRoomNameInput = GetNode<LineEdit>("CreateRoom/VBoxContainer/RoomName");
        _createPasswordInput = GetNode<LineEdit>("CreateRoom/VBoxContainer/Password");
        _createConfirmBtn = GetNode<Button>("CreateRoom/VBoxContainer/Button");
        _createStatus = GetNode<Label>("CreateRoom/VBoxContainer/CreateStatus");

        // Join popup elements
        _joinRoomIdInput = GetNode<LineEdit>("JoinRoom/VBoxContainer/RoomId");
        _joinPasswordInput = GetNode<LineEdit>("JoinRoom/VBoxContainer/Password");
        _joinConfirmBtn = GetNode<Button>("JoinRoom/VBoxContainer/Button");
        _joinStatus = GetNode<Label>("JoinRoom/VBoxContainer/JoinStatus");

        // Connect signals
        _createRoomBtn.Pressed += OnCreateRoomPressed;
        _joinRoomBtn.Pressed += OnJoinRoomPressed;
        _createConfirmBtn.Pressed += OnCreateConfirmPressed;
        _joinConfirmBtn.Pressed += OnJoinConfirmPressed;

        _createRoomPopUp.CloseRequested += OnCreatePopupClosed;
        _joinRoomPopUp.CloseRequested += OnJoinPopupClosed;

        _usernameLabel.Text = $"Welcome, {ClientStateManager.Instance.LocalPlayer.Username}!";
        _idLabel.Text = $"(ID: {ClientStateManager.Instance.LocalPlayer.PlayerId})";
        _status.Text = "Status: Connected";
    }

    private void OnCreateRoomPressed()
    {
        GD.Print($"Create room pressed - popup {_createRoomPopUp}");
        _createRoomNameInput.Text = "";
        _createPasswordInput.Text = "";
        _createRoomPopUp.PopupCentered();
        _status.Text = "Enter room details to create...";
    }

    private void OnJoinRoomPressed()
    {
        GD.Print($"Join room pressed - popup {_joinRoomPopUp}");
        _joinRoomIdInput.Text = "";
        _joinPasswordInput.Text = "";
        _joinRoomPopUp.PopupCentered();
        _status.Text = "Enter room details to join...";
    }

    private void OnCreateConfirmPressed()
    {
        string roomName = _createRoomNameInput.Text.Trim();
        string password = _createPasswordInput.Text.Trim();
        if (string.IsNullOrEmpty(roomName))
        {
            _createStatus.Text = "Status: room's name can not be empty";
            return;
        }
        GD.Print($"Sending create room: {roomName}, Password: {password}");
        NetworkManagement.Instance.CreateRoom(roomName, password);
        _status.Text = "Creating room...";
        _createRoomPopUp.Hide();
    }

    private void OnJoinConfirmPressed()
    {
        string roomName = _joinRoomIdInput.Text.Trim();
        string password = _joinPasswordInput.Text.Trim();
        if (string.IsNullOrEmpty(roomName))
        {
            _joinStatus.Text = "Status: room's name can not be empty";
            return;
        }
        GD.Print($"Sending join room: {roomName}, Password: {password}");
        /*NetworkManagement.Instance.JoinRoom(roomName, password);*/
        _status.Text = "Joining room...";
        _joinRoomPopUp.Hide();
    }

    private void OnCreatePopupClosed()
    {
        _createRoomPopUp.Hide();
    }

    private void OnJoinPopupClosed()
    {
        _joinRoomPopUp.Hide();
    }

    public override void _Process(double delta) { }
}
