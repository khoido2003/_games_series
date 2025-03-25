using Godot;

public class Player
{
    public int PlayerId { get; set; } = -1;
    public string Username { get; set; } = "";
    public Vector2 Position { get; set; } = Vector2.Zero;
    public ulong LastActive { get; set; } = 0;
    public bool Connected { get; set; } = false;

    public Player(int playerId, string username)
    {
        PlayerId = playerId;
        Username = username;
        LastActive = Time.GetTicksMsec();
    }

    public void UpdatePosition(Vector2 newPos)
    {
        Position = newPos;
        LastActive = Time.GetTicksMsec();
    }
}
