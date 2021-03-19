using Godot;
using System;

public class Player : KinematicBody2D
{

    private float _topSpeed = 150f;

    private float _acceleration = 80f;

    private float _fireRate;
    private float _fireRateCounter;
    private Vector2 _movement;
    private Vector2 _fireDirection = Vector2.Right;
    private Vector2 _bulletSpawnOffset = new Vector2(15, 0);

    private PackedScene _bulletScene;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        var singleton = GetNode<Singleton>("/root/Singleton");
        _topSpeed = singleton.PlayerTopSpeed;
        _acceleration = singleton.PlayerAcceleration;
        _fireRate = singleton.PlayerFireRate;
        _fireRateCounter = _fireRate;

        _bulletScene = (PackedScene)ResourceLoader.Load("res://objects/weapon/Bullet.tscn");
    }

    /// <summary>
    /// Handles player sprite movement.
    /// </summary>
    /// <param name="delta">Time elapsed</param>
    private void _handleMovement(float delta)
    {

        var rotation = Convert.ToInt32(Input.IsActionPressed("rotate_right")) - Convert.ToInt32(Input.IsActionPressed("rotate_left"));


        if (rotation != 0)
        {

            _fireDirection = _fireDirection.Rotated(Mathf.Pi * rotation * delta);
        }
        LookAt(GlobalPosition + _fireDirection);

        var directionX = Convert.ToInt32(Input.IsActionPressed("right")) - Convert.ToInt32(Input.IsActionPressed("left"));
        var directionY = Convert.ToInt32(Input.IsActionPressed("down")) - Convert.ToInt32(Input.IsActionPressed("up"));

        var direction = new Vector2(directionX, directionY);

        if (direction.Length() > 0)
        {
            direction = direction.Clamped(1f);
            _movement += direction * _acceleration;
            _movement = _movement.Clamped(_topSpeed);
        }
        else
        {
            if (_movement.x > 0) _movement.x -= _acceleration;
            if (_movement.y > 0) _movement.y -= _acceleration;

            if (_movement.Length() < _acceleration)
            {
                _movement = Vector2.Zero;
            }
            if (_movement.x < 0) _movement.x += _acceleration;
            if (_movement.y < 0) _movement.y += _acceleration;

        }

        MoveAndCollide(_movement * delta);
    }

    /// <summary>
    /// Handling firing bullets and cooldown controlling.
    /// </summary>
    /// <param name="delta">Time elapsed</param>
    public void _handleFiring(float delta)
    {
        if (Input.IsActionPressed("fire"))
        {
            if (_fireRateCounter >= _fireRate)
            {
                var bullet = (Bullet)_bulletScene.Instance();
                GetParent<Node2D>().AddChild(bullet);
                bullet.init(this, _bulletSpawnOffset, _fireDirection);

                var bullet2 = (Bullet)_bulletScene.Instance();
                GetParent<Node2D>().AddChild(bullet2);
                bullet2.init(this, _bulletSpawnOffset, -_fireDirection, Mathf.Pi);

                _fireRateCounter = 0f;
            }
        }
        _fireRateCounter = Math.Min(_fireRate, _fireRateCounter + delta);
    }



    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {

        _handleMovement(delta);
        _handleFiring(delta);
    }
}
