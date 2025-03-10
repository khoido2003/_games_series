using System;
using Godot;

public partial class Line2d : Line2D
{
    public override void _Ready()
    {
        Vector2 screenSize = GetViewport().GetVisibleRect().Size;
        Vector2 top = new(screenSize.X / 2, 0);
        Vector2 bottom = new(screenSize.X / 2, screenSize.Y);

        AddPoint(top);
        AddPoint(bottom);
    }
}
