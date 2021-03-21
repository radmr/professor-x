using Godot;
using System;

public class Title : Control
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    private Label _title;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        var singleton = GetNode<Singleton>("/root/Singleton");
        singleton.SetHUDVisibility(false);

        _title = GetNode<Label>("Title");
        _title.PercentVisible = 0;
    }

    public void _on_Start_pressed()
    {
        GetTree().ChangeScene("res://levels/Level1.tscn");
    }

    public void _on_Mute_pressed()
    {
        var audio = GetNode<AudioStreamPlayer>("/root/Singleton/AudioStreamPlayer");
        audio.Playing = !audio.Playing;
        var muteButton = GetNode<Button>("Buttons/Mute");
        if (audio.Playing)
        {
            muteButton.Text = "Mute";
        }
        else
        {
            muteButton.Text = "Unmute";
        }
    }

    public void _on_Timer_timeout()
    {
        _title.PercentVisible = Mathf.Min(_title.PercentVisible + 0.01f, 100f);
    }
    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
}
