using System;
using Godot;

public partial class Coin : Area2D
{
    public void OnBodyEntered(Node2D body)
    {
        GD.Print("hey there");
        QueueFree();
    }
}
