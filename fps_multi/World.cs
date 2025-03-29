using System;
using Godot;

public partial class World : Node
{
    private CanvasLayer MainMenu;
    private LineEdit AddressEntry;
    private PackedScene PlayerScene;

    public const int PORT = 9999;
    public ENetMultiplayerPeer EnetPeer = new ENetMultiplayerPeer();

    public override void _UnhandledInput(InputEvent @event)
    {
        if (Input.IsActionJustPressed("quit"))
        {
            GetTree().Quit();
        }
    }

    public void OnJoinButtonPressed()
    {
        MainMenu.Hide();
    }

    public void OnHostButtonPress()
    {
        MainMenu.Hide();
        EnetPeer.CreateServer(PORT);
        Multiplayer.MultiplayerPeer = EnetPeer;
        AddPlayer(Multiplayer.GetUniqueId());
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        PlayerScene = (PackedScene)ResourceLoader.Load("res://player.tscn");
        MainMenu = GetNode<CanvasLayer>("CanvasLayer/MainMenu");
        AddressEntry = GetNode<LineEdit>(
            "CanvasLayer/MainMenu/MarginContainer/VBoxContainer/AddressEntry"
        );
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) { }

    public void AddPlayer(int peerId)
    {
        Node player = PlayerScene.Instantiate();
        player.Name = peerId.ToString();
        AddChild(player);
    }
}
