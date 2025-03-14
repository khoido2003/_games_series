using System;
using Godot;

public partial class Slime : Node2D
{
    private const int speed = 60;
    public RayCast2D RayCastRight;
    public RayCast2D RayCastLeft;
    public AnimatedSprite2D animated;
    private int direction = 1;

    public override void _Ready()
    {
        RayCastRight = GetNode<RayCast2D>("RayCastRight");
        RayCastLeft = GetNode<RayCast2D>("RayCastLeft");
         animated = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (RayCastRight.IsColliding())
        {
            direction = -1;
            animated.FlipH = true;
        }

        if (RayCastLeft.IsColliding())
        {
            direction = 1;
            animated.FlipH = false;
        }

        Position = new Vector2(Position.X + (float)(direction * speed * delta), Position.Y);
    }
}
