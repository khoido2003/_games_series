using System;
using Godot;

public partial class UserPaddle : CharacterBody2D
{
    [Export]
    public float speed = 400;

    public Vector2 velocity = Vector2.Zero;
    private RectangleShape2D paddleCollider;

    public override void _Ready()
    {
        paddleCollider = (RectangleShape2D)GetNode<CollisionShape2D>("CollisionShape2D").Shape;
    }

    public override void _PhysicsProcess(double delta)
    {
        float direction = 0;

        if (Input.IsActionPressed("move_up"))
        {
            direction -= 1;
        }

        if (Input.IsActionPressed("move_down"))
        {
            direction += 1;
        }

        Vector2 screenSize = GetViewport().GetVisibleRect().Size;
        float paddleHeight = paddleCollider.Size.Y;

        float minY = paddleHeight / 2;
        float maxY = screenSize.Y - (paddleHeight / 2);

        Position = new Vector2(Position.X, Mathf.Clamp(Position.Y, minY, maxY));

        Velocity = new Vector2(0, direction * speed);
        MoveAndSlide();
    }
}
