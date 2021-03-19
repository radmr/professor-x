using Godot;
using System;

public class Level : Node2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    private PackedScene _mobScene;
    private Player _player;

    public int MobCount { get; set; }
    public override void _Ready()
    {
        MobCount = 20;
        _mobScene = (PackedScene)ResourceLoader.Load("res://objects/character/Zombie.tscn");
        _player = GetNode<Player>("Player");
    }

    public void Spawn()
    {
        var random = new Random();

        int spawnX = random.Next((int)GetViewportRect().Size.x);
        int spawnY = random.Next((int)GetViewportRect().Size.y);

        int edge = random.Next(4);

        switch (edge)
        {
            case 0:
                spawnX = -(int)GetViewportRect().Size.x;
                break;
            case 1:
                spawnX = (int)GetViewportRect().Size.x;
                break;
            case 2:
                spawnY = -(int)GetViewportRect().Size.y;
                break;
            case 3:
                spawnY = (int)GetViewportRect().Size.y;
                break;
        }

        var zombie = (Zombie)_mobScene.Instance();
        zombie.init(_player);
        AddChild(zombie);
        zombie.GlobalPosition = _player.GlobalPosition + new Vector2(spawnX, spawnY);

    }

    public void _on_Timer_timeout()
    {
        if (MobCount > 0)
        {
            Spawn();
            // MobCount--;
        }
        else
        {
            GetNode<Timer>("Timer").Paused = true;
        }
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
}
