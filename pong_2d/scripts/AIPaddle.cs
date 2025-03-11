using System;
using Godot;

public partial class AIPaddle : CharacterBody2D
{
    [Export]
    public float speed = 400;

    [Export]
    public NodePath BallPath;
    private RectangleShape2D paddleCollider;
    private Ball ball;

    public override void _Ready()
    {
        paddleCollider = (RectangleShape2D)GetNode<CollisionShape2D>("CollisionShape2D").Shape;

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
        // Move the paddle
        float moveAmount = direction * speed * (float)delta;
        Position += new Vector2(0, moveAmount);

        Vector2 screenSize = GetViewport().GetVisibleRect().Size;
        float paddleHeight = paddleCollider.Size.Y;

        float minY = paddleHeight / 2;
        float maxY = screenSize.Y - (paddleHeight / 2);

        Position = new Vector2(Position.X, Mathf.Clamp(Position.Y, minY, maxY));
    }
}
