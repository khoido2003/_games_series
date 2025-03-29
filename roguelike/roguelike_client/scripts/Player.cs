using System;
using System.Collections.Generic;
using Godot;

public partial class Player : CharacterBody2D
{
    public const float Speed = 200.0f;

    public int PlayerId { get; set; } = -1;
    public string Username { get; set; } = "";
    public ulong LastActive { get; set; } = 0;
    public bool Connected { get; set; } = false;

    private int _inputSequence = 0;
    private Queue<(int sequence, Vector2 position, Vector2 velocity)> _predictedStates =
        new Queue<(int, Vector2, Vector2)>();

    public Player(int playerId, string username)
    {
        PlayerId = playerId;
        Username = username;
        LastActive = Time.GetTicksMsec();
    }

    public Player() { }

    public override void _Ready()
    {
        GD.Print("Player initialized with ID: ", PlayerId);
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector2 velocity = Velocity;

        // Get the input direction and handle the movement/deceleration.
        Vector2 direction = Input.GetVector("left", "right", "up", "down");
        GD.Print("Input direction: ", direction);

        if (direction != Vector2.Zero)
        {
            velocity.X = direction.X * Speed;
            velocity.Y = direction.Y * Speed;
            GD.Print("Moving with velocity: ", velocity);
        }
        else
        {
            velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
            velocity.Y = Mathf.MoveToward(velocity.Y, 0, Speed);
        }

        Velocity = velocity;
        MoveAndSlide();

        // Debug current position and velocity
        GD.Print("Position: ", Position, " Velocity: ", Velocity);
    }
}
