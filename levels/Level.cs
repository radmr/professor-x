using Godot;
using System;

public class Level : Node2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";


    private int _level;
    private PackedScene _mobScene;
    private Player _player;


    [Signal] public delegate void UpdateMob(int count);
    public int MobCount { get; set; }
    public override void _Ready()
    {
        var singleton = GetNode<Singleton>("/root/Singleton");
        singleton.SetHUDVisibility(true);
        singleton.UpdateLevel();
        singleton.SpeedrunActive = true;
        MobCount = singleton.LevelNumber * 5;
        _level = singleton.LevelNumber;
        _mobScene = (PackedScene)ResourceLoader.Load("res://objects/character/Zombie.tscn");
        _player = GetNode<Player>("Player");

        Connect("UpdateMob", singleton, "UpdateMobCount");
    }

    public void SpawnMob()
    {
        var spawners = GetNode<Node>("Spawners").GetChildren();
        var random = new Random();

        if (spawners.Count > 0)
        {
            if ((Spawner)spawners[random.Next(spawners.Count)] is Spawner spawner)
            {
                spawner.init(_player, GetNode<Node>("Mobs"));
                spawner.Spawn();
            }

        }

    }

    public void _on_Timer_timeout()
    {
        if (MobCount > 0)
        {
            SpawnMob();
            MobCount--;
        }
        else
        {
            GetNode<Timer>("Timer").Paused = true;
        }
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        var zomebieCount = 0;
        float minDistance = 0;
        foreach (Node children in GetNode("Mobs").GetChildren())
        {
            if (children is Zombie zombie)
            {
                zomebieCount++;
                if (minDistance == 0f || (zombie.GlobalPosition - _player.GlobalPosition).Length() < minDistance)
                {
                    minDistance = (zombie.GlobalPosition - _player.GlobalPosition).Length();
                    _player.UpdateIndicator(zombie);
                }
            }
        }
        EmitSignal("UpdateMob", zomebieCount + MobCount);
        if (zomebieCount == 0 && MobCount == 0)
        {
            if (_level > 12)
            {
                GetTree().ChangeScene("res://ui/Credits.tscn");

            }
            else
            {

                GetTree().ChangeScene("res://ui/Evolve.tscn");
            }
        }
    }
}
