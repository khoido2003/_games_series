using System;
using Godot;

public partial class ScoreManager : Label
{
    private int playerScore = 0;
    private int aiScore = 0;

    public void OnBallUpdateScore(string side)
    {
        if (side == "Player")
        {
            playerScore++;
        }
        else if (side == "AI")
        {
            aiScore++;
        }

        GD.Print($"Player: {playerScore} | AI: {aiScore}");

        Text = $"{playerScore} | {aiScore}";
    }
}
