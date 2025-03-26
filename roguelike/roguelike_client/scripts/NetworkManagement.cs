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

    public override void _ExitTree()
    {
        DisconnectToServer();
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
        SendHandshake(username);
    }

    /////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////

    // ------ Action Method -------

    private void SendHandshake(string username)
    {
        // Send command + name length + name
        byte[] usernameBytes = Encoding.UTF8.GetBytes(username);
        byte[] packet = new byte[3 + usernameBytes.Length];
        packet[0] = 1;

        ushort length = (ushort)usernameBytes.Length;
        BitConverter.GetBytes(length).CopyTo(packet, 1);

        Array.Copy(usernameBytes, 0, packet, 3, usernameBytes.Length);
        _networkClient.SendPacket(packet);
        GD.Print($"Sent Handshake with username: {username} (length: {length})");
    }

    public void DisconnectToServer()
    {
        if (ClientStateManager.Instance.LocalPlayer.Connected)
        {
            byte[] leave = new byte[5];
            leave[0] = (byte)Global.LEAVE;
            BitConverter
                .GetBytes(ClientStateManager.Instance.LocalPlayer.PlayerId)
                .CopyTo(leave, 1);

            _networkClient.SendPacket(leave);

            GD.Print($"Sent LEAVE message");

            // Delay the server to send the message before close udp
            OS.DelayMsec(100);
        }

        NetworkManagement.Instance.UpdateConnectionStatus(false, -1);
        _networkClient.CloseUdp();
    }

    public void CreateRoom(string roomName, string password)
    {
        byte[] nameBytes = Encoding.UTF8.GetBytes(roomName);
        byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

        byte[] msg = new byte[5 + nameBytes.Length + passwordBytes.Length];
        msg[0] = (byte)Global.CREATE_ROOM;

        // Put the name bytes length to the index 1 and the rest of the names from index 3
        BitConverter.GetBytes((ushort)nameBytes.Length).CopyTo(msg, 1);
        Array.Copy(nameBytes, 0, msg, 3, nameBytes.Length);

        // Put the password bytes length to the index 3
        BitConverter.GetBytes((ushort)passwordBytes.Length).CopyTo(msg, 3 + nameBytes.Length);
        Array.Copy(passwordBytes, 0, msg, 3 + nameBytes.Length + 2, passwordBytes.Length);

        _networkClient.SendPacket(msg);
        GD.Print(
            $"Send CREATE_ROOM: name={roomName}, password={password}, packet length={msg.Length}"
        );
    }

    public void JoinRoom(int roomId, string password)
    {
        byte[] idBytes = BitConverter.GetBytes(roomId);
        byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

        byte[] msg = new byte[5 + idBytes.Length + passwordBytes.Length];
        msg[0] = (byte)Global.JOIN_ROOM;

        // Put the name bytes length to the index 1 and the rest of the names from index 3
        BitConverter.GetBytes((ushort)idBytes.Length).CopyTo(msg, 1);
        Array.Copy(idBytes, 0, msg, 3, idBytes.Length);

        // Put the password bytes length to the index 3
        BitConverter.GetBytes((ushort)passwordBytes.Length).CopyTo(msg, 3 + idBytes.Length);
        Array.Copy(passwordBytes, 0, msg, 3 + idBytes.Length + 2, passwordBytes.Length);

        _networkClient.SendPacket(msg);
        GD.Print($"Send JOIN_ROOM: ID={roomId}, password={password}, packet length={msg.Length}");
    }
}
