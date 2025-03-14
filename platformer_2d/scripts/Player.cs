using System;
using Godot;

public partial class Player : CharacterBody2D
{
    public const float Speed = 130.0f;
    public const float JumpVelocity = -300.0f;
    public AnimatedSprite2D animated;

    public override void _Ready()
    {
        animated = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector2 velocity = Velocity;

        // Add the gravity.
        if (!IsOnFloor())
        {
            velocity += GetGravity() * (float)delta;
        }

        // Handle Jump.
        if (Input.IsActionJustPressed("jump") && IsOnFloor())
        {
            velocity.Y = JumpVelocity;
        }

        Vector2 direction = new Vector2(Input.GetAxis("move_left", "move_right"), 0);

        // Change the side look
        if (direction.X >= 0)
        {
            animated.FlipH = false;
        }
        else
        {
            animated.FlipH = true;
        }

        // Animation
        if (IsOnFloor())
        {
            if (direction.X == 0)
            {
                animated.Play("idle");
            }
            else
            {
                animated.Play("run");
            }
        }
        else
        {
            animated.Play("jump");
        }

        // Move
        if (direction != Vector2.Zero)
        {
            velocity.X = direction.X * Speed;
        }
        else
        {
            velocity.X = Mathf.MoveToward(velocity.X, 0, Speed);
        }
        Velocity = velocity;
        MoveAndSlide();
    }
}
