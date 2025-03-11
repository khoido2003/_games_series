using System;
using Godot;

public partial class Ball : CharacterBody2D
{
    [Signal]
    public delegate void UpdateScoreEventHandler(string side);

    private float speed = 300;
    private Vector2 velocity = Vector2.Zero;

    private enum NextDirection
    {
        LEFT,
        RIGHT,
    }

    public override void _Ready()
    {
        Position = GetViewport().GetVisibleRect().Size / 2;
        StartBall();
    }

    public override void _PhysicsProcess(double delta)
    {
        KinematicCollision2D collision = MoveAndCollide(velocity * (float)delta);

        if (collision != null)
        {
            var collider = collision.GetCollider();
            if (collider is UserPaddle || collider is AIPaddle)
            {
                speed *= 1.3f; // Increase speed
            }
            // Make the ball bounce when have collision to something
            velocity = velocity.Bounce(collision.GetNormal()).Normalized() * speed;
        }

        // Calculate the current screen size
        Vector2 screenSize = GetViewport().GetVisibleRect().Size;

        // Calculate the ball radius
        float ballRadius = (
            (CircleShape2D)GetNode<CollisionShape2D>("CollisionShape2D").Shape
        ).Radius;

        if (Position.Y - ballRadius <= 0 || Position.Y + ballRadius >= screenSize.Y)
        {
            velocity.Y = -velocity.Y;
        }

        if (Position.X - ballRadius <= 0 || Position.X + ballRadius >= screenSize.X)
        {
            Visible = false;

            if (Position.X - ballRadius <= 0)
            {
                EmitSignal(SignalName.UpdateScore, "AI");
                ResetBall(NextDirection.RIGHT);
            }
            else
            {
                EmitSignal(SignalName.UpdateScore, "Player");
                ResetBall(NextDirection.LEFT);
            }
        }
    }

    private void StartBall()
    {
        float directionX = GD.Randf() > 0.5 ? 1 : -1;
        float directionY = (float)GD.RandRange(-0.5f, 0.5f);

        velocity = new Vector2(directionX, directionY).Normalized() * speed;

        // Create a dealy timer before display the ball
        GetTree().CreateTimer(1.0f).Timeout += () => Visible = true;
    }

    private void ResetBall(NextDirection direction)
    {
        velocity = Vector2.Zero;
        speed = 300;
        GetTree().CreateTimer(0.5f).Timeout += () =>
        {
            Vector2 screenSize = GetViewport().GetVisibleRect().Size;
            Position = screenSize / 2; // Ensure position is centered at reset
            Visible = true;
        };

        float directionX = direction == NextDirection.LEFT ? -1 : 1;
        float directionY = (float)GD.RandRange(-0.5f, 0.5f);

        velocity = new Vector2(directionX, directionY).Normalized() * speed;
    }
}
