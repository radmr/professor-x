using Godot;
using System;

public class Exit : Node2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    [Signal] public delegate void PlayerEntered();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Connect("PlayerEntered", GetParent(), "CompleteLevel");

    }

    public void _on_Area2D_body_entered(PhysicsBody2D body)
    {
        if (body is Player player)
        {
            EmitSignal("PlayerEntered");
        }
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
}
