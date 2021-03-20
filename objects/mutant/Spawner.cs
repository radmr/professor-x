using Godot;
using System;

public class Spawner : Node2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    private PackedScene _mobScene;
    private Player _player;
    private Node _spawnNode;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _mobScene = (PackedScene)ResourceLoader.Load("res://objects/character/Zombie.tscn");

    }

    public void init(Player player, Node spawnNode)
    {
        _player = player;
        _spawnNode = spawnNode;
    }


    public void Spawn()
    {
        var zombie = (Zombie)_mobScene.Instance();
        zombie.init(_player);
        _spawnNode.AddChild(zombie);
        zombie.GlobalPosition = GlobalPosition;
    }
    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
}
