using Godot;
using System;

public class Singleton : Node
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    public float PlayerTopSpeed { get; set; }

    public float PlayerAcceleration { get; set; }

    public float PlayerFireRate { get; set; }

    public float BulletScale { get; set; }


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {

        PlayerTopSpeed = 200f;
        PlayerAcceleration = 20f;
        PlayerFireRate = 0.1f;
        BulletScale = 0.5f;
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
}
