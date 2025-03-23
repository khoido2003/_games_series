using System;
using System.Text;
using Godot;

public partial class NetworkClient : Node
{
    public delegate void PacketHandler(byte[] packet);
    public event PacketHandler OnPacketReceived;

    private PacketPeerUdp _udp = new PacketPeerUdp();
    private const string SERVER_IP = "127.0.0.1";
    private const int SERVER_PORT = 8012;
    private const int CLIENT_PORT = 123456;

    private string _username = "";
    public int PlayerId { get; private set; } = -1;
    public bool Connected { get; private set; } = false;

    public override void _Ready()
    {
        Error err = _udp.ConnectToHost(SERVER_IP, SERVER_PORT);

        if (err != Error.Ok)
        {
            GD.PrintErr($"Failed to connect to {SERVER_IP}:{SERVER_PORT}: {err}");
            return;
        }

        GD.Print($"Client bound to {CLIENT_PORT}, connect to {SERVER_IP}:{SERVER_PORT}");
    }

    public override void _EnterTree()
    {
        DisconnectServer();
    }

    private void SendHandshake()
    {
        // Send command + name length + name
        byte[] usernameBytes = Encoding.UTF8.GetBytes(_username);
        byte[] packet = new byte[3 + usernameBytes.Length];
        packet[0] = 1;

        ushort length = (ushort)usernameBytes.Length;
        BitConverter.GetBytes(length).CopyTo(packet, 1);

        Array.Copy(usernameBytes, 0, packet, 3, usernameBytes.Length);
        _udp.PutPacket(packet);

        GD.Print($"Sent Handshake with username: {_username} (length: {length})");
    }

    public void ConnectToServer(string username)
    {
        _username = username;
        SendHandshake();
    }

    public override void _Process(double delta)
    {
        /*if (!Connected && _username != "")*/
        /*{*/
        /*    SendHandshake();*/
        /*}*/

        if (_udp.GetAvailablePacketCount() > 0)
        {
            byte[] packet = _udp.GetPacket();

            // Read the message sent back from the server
            if (packet[0] == 0)
            {
                GD.Print("Received PING");
                byte[] pong = new byte[] { 0 };
                _udp.PutPacket(pong);
                GD.Print("Send PONG to the server");
            }

            OnPacketReceived?.Invoke(packet);
        }
    }

    public void SendPacket(byte[] packet)
    {
        _udp.PutPacket(packet);
    }

    public void SetConnected(int playerId)
    {
        Connected = true;
        GD.Print($"Connected! PlayerId: {playerId}");
    }

    public void DisconnectServer()
    {
        _udp.Close();
        Connected = false;
    }
}
