using Godot;
using System;

public class Zombie : KinematicBody2D
{
    // Declare member variables here. Examples:
    private float _speed = 40f;

    public int Damage { get; set; }

    private Node2D _target;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        var singleton = GetNode<Singleton>("/root/Singleton");

        _speed = Mathf.CeilToInt((float)singleton.LevelNumber / 2) * 30f;
        Damage = Mathf.CeilToInt((float)singleton.LevelNumber / 3) * 5;

    }

    public void init(Node2D target)
    {
        _target = target;
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        LookAt(_target.GlobalPosition);
        var direction = (_target.GlobalPosition - GlobalPosition).Normalized();
        MoveAndCollide(direction * _speed * delta);
    }
}
