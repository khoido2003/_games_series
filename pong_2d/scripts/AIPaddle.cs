using System;
using Godot;

public partial class AIPaddle : CharacterBody2D
{
    [Export]
    public float Speed = 300;

    [Export]
    public NodePath BallPath;

    private Ball ball;

    public override void _Ready()
    {
        // Find the ball
        ball = GetNode<Ball>(BallPath);

        if (ball == null)
        {
            GD.Print("Error: Ball not found!");
        }
        else
        {
            GD.Print("Ball found at: ", ball.Position);
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        // Get the ball at y coordination
        float ballY = ball.Position.Y;

        // Get The paddle Y coordination
        float paddleY = Position.Y;

        // Try to move toward the ball Y position
        float direction = 0;

        if (ballY < paddleY - 10)
        {
            direction -= 1;
        }

        if (ballY > paddleY + 10)
        {
            direction += 1;
        }

        Velocity = new Vector2(0, direction * Speed);
        MoveAndSlide();
    }
}
