using Godot;
using System;

public class Pause : Control
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {

    }

    public void init(string message)
    {
        GetNode<Label>("VBox/Status").Text = message;
    }

    public void disableButton(string path)
    {
        GetNode<Button>("VBox/" + path).Disabled = true;
    }
    public void _on_Resume_pressed()
    {
        GetTree().Paused = false;
        Hide();
    }

    public void _on_Restart_pressed()
    {
        Hide();
        GetTree().Paused = false;
        var level = GetNode<Singleton>("/root/Singleton").LevelNumber;
        GetTree().ChangeScene($"res://levels/Level{level}.tscn");

    }

    public void _on_Menu_pressed()
    {
        Hide();
        GetTree().Paused = false;
        GetTree().ChangeScene($"res://ui/Title.tscn");
    }
    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
}
