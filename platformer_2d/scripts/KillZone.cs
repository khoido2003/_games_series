using System;
using Godot;

public partial class KillZone : Area2D
{
    private Timer _timer;

    public override void _Ready()
    {
        _timer = GetNode<Timer>("Timer");
    }

    public void OnBodyEntered(Node2D body)
    {
        GD.Print("You die");
        _timer.Start();
    }

    private void OnTimeOut()
    {
        GD.Print("Die");
        GetTree().ReloadCurrentScene();
    }
}
