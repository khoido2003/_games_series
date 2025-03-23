using System;
using System.Text;
using Godot;

public partial class NetworkClient : Node
{
    public enum MessageType : byte
    {
        PING = 0,
        HANDSHAKE = 1,
        ACK = 2,
        LEAVE = 3,
    }

    public delegate void PacketHandler(byte[] packet);
    public event PacketHandler OnPacketReceived;

    private PacketPeerUdp _udp = new PacketPeerUdp();
    private const string SERVER_IP = "127.0.0.1";
    private const int SERVER_PORT = 8012;
    private const int CLIENT_PORT = 123456;

    private string _username = "";
    private NetworkManagement _networkManagement;

    public override void _Ready()
    {
        _networkManagement = GetParent<NetworkManagement>();
        Error err = _udp.ConnectToHost(SERVER_IP, SERVER_PORT);

        if (err != Error.Ok)
        {
            GD.PrintErr($"Failed to connect to {SERVER_IP}:{SERVER_PORT}: {err}");
            return;
        }

        GD.Print($"Client bound to {CLIENT_PORT}, connect to {SERVER_IP}:{SERVER_PORT}");
    }

    public override void _ExitTree()
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
        if (_udp.GetAvailablePacketCount() > 0)
        {
            byte[] packet = _udp.GetPacket();

            // Read the message sent back from the server then send PING back to the server to make sure the server does not remove this client from the server
            switch ((MessageType)packet[0])
            {
                case MessageType.PING:
                    GD.Print("Received PING from the server");

                    byte[] pong = new byte[] { (byte)MessageType.PING };
                    _udp.PutPacket(pong);

                    GD.Print("SEND PONG message back to the server");
                    break;

                case MessageType.ACK:
                    if (packet.Length >= 5)
                    {
                        int playerId = BitConverter.ToInt32(packet, 1);
                        GD.Print($"ACK mes received - PlayerId is {playerId}");

                        _networkManagement.UpdateConnectionStatus(true, playerId);

                        GD.Print(
                            $"Emitting ConnectionStatusChanged: Connected={_networkManagement.Connected}, PlayerId={_networkManagement.PlayerId}"
                        );
                    }
                    break;

                case MessageType.LEAVE:
                    if (packet.Length >= 5)
                    {
                        int playerId = BitConverter.ToInt32(packet, 1);
                        GD.Print($"Player {playerId} left the server");
                    }
                    break;

                default:
                    GD.Print($"Unknown MessageType: {packet[0]}");
                    break;
            }

            OnPacketReceived?.Invoke(packet);
        }
    }

    public void SendPacket(byte[] packet)
    {
        _udp.PutPacket(packet);
    }

    public void DisconnectServer()
    {
        if (_networkManagement.Connected)
        {
            byte[] leave = new byte[5];
            leave[0] = (byte)MessageType.LEAVE;
            BitConverter.GetBytes(_networkManagement.PlayerId).CopyTo(leave, 1);

            _udp.PutPacket(leave);
            GD.Print($"Sent LEAVE message");
            
            // Delay the server to send the message before close udp
            OS.DelayMsec(100);
        }
        _networkManagement.UpdateConnectionStatus(false, -1);
        _udp.Close();
    }
}
