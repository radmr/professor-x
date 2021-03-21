using Godot;
using System;

/// <summary>
/// Class for player-controlled character.
/// </summary>
public class Player : KinematicBody2D
{

    // Properties tracked by autoload.
    private int _health;
    private float _topSpeed;
    private float _acceleration;
    private float _fireRate;
    private float _fireRateCounter;
    private Singleton.FiringMode _firingMode;
    private int _evolution;
    private bool _hitCooldown;


    // Local properties
    private Vector2 _movement;
    private Vector2 _fireDirection = Vector2.Right;
    private Vector2 _bulletSpawnOffset = new Vector2(15, 0);
    private PackedScene _bulletScene;


    // Children nodes
    private AnimatedSprite _animatedSprite;
    private Area2D _hitbox;
    private Timer _timer;
    // Signals
    [Signal] public delegate void UpdateHUDHealth(int currentHealth);

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");

        var singleton = GetNode<Singleton>("/root/Singleton");
        _topSpeed = singleton.PlayerTopSpeed;
        _acceleration = singleton.PlayerAcceleration;
        _fireRate = singleton.PlayerFireRate;
        _fireRateCounter = _fireRate;
        _firingMode = singleton.PlayerFiringMode;
        _health = singleton.PlayerMaxHealth;
        _evolution = singleton.PlayerEvolution;
        _hitCooldown = true;

        Connect("UpdateHUDHealth", singleton, "UpdateHealth");

        switch (_firingMode)
        {
            case Singleton.FiringMode.MONO:
                _animatedSprite.Play("default");
                break;
            case Singleton.FiringMode.BI:
                _animatedSprite.Play("double");
                break;
            case Singleton.FiringMode.QUAD:
                _animatedSprite.Play("quad");
                break;
        }


        _bulletScene = (PackedScene)ResourceLoader.Load("res://objects/weapon/Bullet.tscn");
        _hitbox = GetNode<Area2D>("Hitbox");
        _timer = GetNode<Timer>("Timer");
    }

    /// <summary>
    /// Handles <see cref="Player"/> sprite movement.
    /// </summary>
    /// <param name="delta">Time elapsed</param>
    private void _handleMovement(float delta)
    {


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
                switch (_firingMode)
                {
                    case Singleton.FiringMode.MONO:
                        var bullet = (Bullet)_bulletScene.Instance();
                        GetParent<Node2D>().AddChild(bullet);
                        bullet.init(this, _bulletSpawnOffset, _fireDirection);
                        break;

                    case Singleton.FiringMode.BI:
                        for (int i = 0; i < 2; i++)
                        {
                            var biBullet = (Bullet)_bulletScene.Instance();
                            GetParent<Node2D>().AddChild(biBullet);
                            biBullet.init(this, _bulletSpawnOffset, _fireDirection, i * Mathf.Pi);
                        }
                        break;
                    case Singleton.FiringMode.QUAD:
                        for (int i = 0; i < 4; i++)
                        {
                            var quadBullet = (Bullet)_bulletScene.Instance();
                            GetParent<Node2D>().AddChild(quadBullet);
                            quadBullet.init(this, _bulletSpawnOffset, _fireDirection, i * Mathf.Pi / 2);
                        }
                        break;
                }

                _fireRateCounter = 0f;
            }
        }
        _fireRateCounter = Math.Min(_fireRate, _fireRateCounter + delta);
    }

    private void _handleRotation(float delta)
    {
        if (_evolution <= 3)
        {
            if (_movement.Length() > _acceleration)
            {
                _fireDirection = _movement.Normalized();
            }
        }
        else if (_evolution > 3 && _evolution < 6)
        {
            if (Input.IsActionJustPressed("rotate_right"))
            {
                _fireDirection = _fireDirection.Rotated(Mathf.Pi / 2);

            }
            else if (Input.IsActionJustPressed("rotate_left"))
            {
                _fireDirection = _fireDirection.Rotated(-Mathf.Pi / 2);
            }

        }
        else
        {

            var rotation = Convert.ToInt32(Input.IsActionPressed("rotate_right")) - Convert.ToInt32(Input.IsActionPressed("rotate_left"));
            if (rotation != 0)
            {

                _fireDirection = _fireDirection.Rotated(Mathf.Pi * rotation * delta);
            }
        }
        LookAt(GlobalPosition + _fireDirection);
    }

    public void TakeDamage(int rawDamage)
    {
        _health -= rawDamage;
        EmitSignal("UpdateHUDHealth", _health);

    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {

        _handleMovement(delta);
        _handleFiring(delta);
        _handleRotation(delta);

        if (_hitCooldown)
        {
            var bodies = _hitbox.GetOverlappingBodies();
            int totalDamage = 0;
            foreach (object body in bodies)
            {

                if (body is Zombie zombie)
                {
                    totalDamage += zombie.Damage;
                }
            }
            TakeDamage(totalDamage);

            _hitCooldown = false;
            _timer.Start();
        }
    }

    public void _on_Timer_timeout()
    {
        _hitCooldown = true;
        _timer.Stop();
    }

    public void UpdateIndicator(Node2D target)
    {
        var indicator = GetNode<Sprite>("Sprite");
        if ((target.GlobalPosition - GlobalPosition).Length() < GetViewportRect().Size.y / 2)
        {
            indicator.Visible = false;
        }
        else
        {
            indicator.Visible = true;
            indicator.LookAt(target.GlobalPosition);
            indicator.Position = new Vector2(20, 0).Rotated(indicator.Rotation);

        }
    }
}
