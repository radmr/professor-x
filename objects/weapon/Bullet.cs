using Godot;
using System;

public class Bullet : Node2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    private float _speed = 500f;

    private Node2D _parent;

    private Area2D _hitbox;
    private Sprite _sprite;

    private Vector2 _projectile;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _hitbox = GetNode<Area2D>("Area2D");
        _sprite = GetNode<Sprite>("Sprite");
        var root = GetNode<Singleton>("/root/Singleton");

        _hitbox.Scale = new Vector2(root.BulletScale, root.BulletScale);
        _sprite.Scale = new Vector2(root.BulletScale, root.BulletScale);
    }

    public void init(Node2D parent, Vector2 offset, Vector2 projectile, float rotateModifier = 0)
    {
        _parent = parent;
        GlobalPosition = parent.GlobalPosition + offset.Rotated(parent.Rotation + rotateModifier);
        _projectile = projectile;
        LookAt(GlobalPosition + _projectile);
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        GlobalPosition += _projectile * _speed * delta;
        _outOfBoundsCheck();
    }

    public void _outOfBoundsCheck()
    {
        if (GlobalPosition.x > GetViewportRect().Size.x + _parent.GlobalPosition.x)
        {
            QueueFree();

        }
        else if (GlobalPosition.x < _parent.GlobalPosition.x - GetViewportRect().Size.x)
        {
            QueueFree();
        }
        else if (GlobalPosition.y < _parent.GlobalPosition.y - GetViewportRect().Size.y)
        {
            QueueFree();
        }
        else if (GlobalPosition.y > GetViewportRect().Size.y + _parent.GlobalPosition.y)
        {
            QueueFree();
        }
    }

    public void _on_Area2D_body_entered(PhysicsBody2D body)
    {
        if (body is Zombie zombie)
        {
            zombie.QueueFree();
            QueueFree();
        }
    }
}
