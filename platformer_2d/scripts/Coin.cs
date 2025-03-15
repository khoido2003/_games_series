using System;
using Godot;

public partial class Coin : Area2D
{
    private GameManager gameManager;
    private AnimationPlayer animationPlayer;

    public override void _Ready()
    {
        gameManager = GetNode<GameManager>("%GameManager");
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
    }

    public void OnBodyEntered(Node2D body)
    {
        gameManager.AddPoint();
        GD.Print("+1 coin");

        /*animationPlayer.Play();*/
        QueueFree();
    }
}
