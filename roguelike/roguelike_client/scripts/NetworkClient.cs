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

    public override void _Process(double delta)
    {
        if (_udp.GetAvailablePacketCount() > 0)
        {
            byte[] packet = _udp.GetPacket();

            // Read the message sent back from the server then send PING back to the server to make sure the server does not remove this client from the server
            switch ((Global)packet[0])
            {
                case Global.ERROR:

                    if (packet.Length >= 3)
                    {
                        int errorLength = BitConverter.ToInt16(packet, 1);

                        if (packet.Length >= 3 + errorLength)
                        {
                            string errorMsg = Encoding.UTF8.GetString(packet, 3, errorLength);

                            GD.PrintErr($"Server Error: {errorMsg}");
                        }
                        else
                        {
                            GD.PrintErr($"ERROR packet too short for message {packet.Length}");
                        }
                    }
                    else
                    {
                        GD.PrintErr($"Error packet too short: {packet.Length}");
                    }
                    break;

                case Global.PING:
                    GD.Print("Received PING from the server");

                    byte[] pong = new byte[] { (byte)Global.PING };
                    _udp.PutPacket(pong);

                    GD.Print("SEND PONG message back to the server");
                    break;

                case Global.ACK:
                    if (packet.Length >= 5)
                    {
                        int playerId = BitConverter.ToInt32(packet, 1);
                        GD.Print($"ACK mes received - PlayerId is {playerId}");

                        NetworkManagement.Instance.UpdateConnectionStatus(true, playerId);

                        GD.Print(
                            $"Emitting ConnectionStatusChanged: Connected={ClientStateManager.Instance.LocalPlayer.Connected}, PlayerId={ClientStateManager.Instance.LocalPlayer.PlayerId}"
                        );
                    }
                    break;

                case Global.LEAVE:
                    if (packet.Length >= 5)
                    {
                        int playerId = BitConverter.ToInt32(packet, 1);
                        GD.Print($"Player {playerId} left the server");
                    }
                    break;

                case Global.CREATE_ROOM:
                    if (packet.Length >= 5)
                    {
                        int roomId = BitConverter.ToInt32(packet, 1);
                        GD.Print($"RoomId {roomId} created");
                    }
                    break;

                case Global.JOIN_ROOM:
                    if (packet.Length >= 3)
                    {
                        try
                        {
                            int nameLength = BitConverter.ToInt32(packet, 1);
                            string roomName = Encoding.UTF8.GetString(packet, 5, nameLength);

                            GD.Print($"Successfulled joined room: {roomName}");
                        }
                        catch (Exception e)
                        {
                            GD.PrintErr($"Error processing JOIN_ROOM response: {e}");
                        }
                    }
                    break;

                default:
                    GD.Print($"Unknown Global: {packet[0]}");
                    break;
            }

            OnPacketReceived?.Invoke(packet);
        }
    }

    public void SendPacket(byte[] packet)
    {
        _udp.PutPacket(packet);
    }

    public void CloseUdp()
    {
        _udp.Close();
    }
}
