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

        body.GetNode<CollisionShape2D>("CollisionShape2D").QueueFree();

        Engine.TimeScale = 0.5;
        _timer.Start();
    }

    private void OnTimeOut()
    {
        GD.Print("Die");
        Engine.TimeScale = 1;
        GetTree().ReloadCurrentScene();
    }
}
