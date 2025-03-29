using System;
using Godot;

public partial class Player : CharacterBody3D
{
    public const float Speed = 10.0f;
    public const float JumpVelocity = 10.0f;
    public const float Gravity = 20.0f;
    private Camera3D camera3D;
    private AnimationPlayer animation;
    private GpuParticles3D muzzleFlash;

    public override void _Ready()
    {
        Input.MouseMode = Input.MouseModeEnum.Captured;
        camera3D = GetNode<Camera3D>("%Camera3D");
        animation = GetNode<AnimationPlayer>("Camera3D/Pistol/AnimationPlayer");
        muzzleFlash = GetNode<GpuParticles3D>("Camera3D/Pistol/MuzzleFlash");
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseMotion motion)
        {
            RotateY(-motion.Relative.X * 0.005f);
            camera3D.RotateX(-motion.Relative.Y * 0.005f);
            camera3D.Rotation = new Vector3(
                Mathf.Clamp(camera3D.Rotation.X, -Mathf.Pi / 2, Mathf.Pi / 2),
                camera3D.Rotation.Y,
                camera3D.Rotation.Z
            );
        }

        if (Input.IsActionJustPressed("shoot") && animation.CurrentAnimation != "shoot")
        {
            animation.Stop();
            animation.Play("shoot");
            muzzleFlash.Restart();
            muzzleFlash.Emitting = true;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector3 velocity = Velocity;

        // Add the gravity.
        if (!IsOnFloor())
        {
            velocity.Y = velocity.Y - (Gravity * (float)delta);
        }

        // Handle Jump.
        if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
        {
            velocity.Y = JumpVelocity;
        }

        // Get the input direction and handle the movement/deceleration.
        // As good practice, you should replace UI actions with custom gameplay actions.
        Vector2 inputDir = Input.GetVector("left", "right", "up", "down");

        if (inputDir != Vector2.Zero && IsOnFloor())
        {
            animation.Play("move");
        }
        else
        {
            animation.Play("idle");
        }

        Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
        if (direction != Vector3.Zero)
        {
            velocity.X = direction.X * Speed;
            velocity.Z = direction.Z * Speed;
        }
        else
        {
            velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
            velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
        }

        Velocity = velocity;
        MoveAndSlide();
    }
}
