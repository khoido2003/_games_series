using System;
using Godot;

public partial class GameManager : Node
{
    private int score = 0;
    private Label currentScoreLabel;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        currentScoreLabel = GetNode<Label>("ScoreLabel");
    }

    public void AddPoint()
    {
        score += 1;
        GD.Print(score);
        currentScoreLabel.Text = "You collected " + score + " coins";
    }
}
