using Godot;

public class InputAction
{
    public int Sequence { get; set; }
    public Vector2 Direction { get; set; }
    public ulong TimeStamp { get; set; }

    public InputAction(int sequence, Vector2 direction)
    {
        Sequence = sequence;
        Direction = direction;
        TimeStamp = Time.GetTicksMsec();
    }
}
