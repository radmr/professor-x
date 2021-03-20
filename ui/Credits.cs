using Godot;
using System;

public class Credits : Control
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    private Godot.Collections.Dictionary _data;
    private Godot.Collections.Dictionary _ending;
    private Godot.Collections.Dictionary _credits;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {

        var singleton = GetNode<Singleton>("/root/Singleton");
        singleton.SpeedrunActive = false;

        var file = new Godot.File();

        file.Open("res://global/data.json", Godot.File.ModeFlags.Read);
        _data = JSON.Parse(file.GetAsText()).Result as Godot.Collections.Dictionary;
        _ending = _data["ending"] as Godot.Collections.Dictionary;
        _credits = _data["credits"] as Godot.Collections.Dictionary;
        file.Close();

        printEndingAndCredits();
    }

    private void printEndingAndCredits()
    {
        var endingLabel = GetNode<Label>("VBox/Ending");
        var singleton = GetNode<Singleton>("/root/Singleton");

        var ending = "0";

        foreach (string level in _ending.Keys)
        {
            if (singleton.PlayerEvolution >= Convert.ToInt32(level))
            {
                ending = level;
            }
        }

        endingLabel.Text = _ending[ending].ToString();

        var creditLabel = GetNode<Label>("VBox/Credits");
        creditLabel.Text = $"{_credits["jam"]}\n{_credits["author"]}\n{_credits["repo"]}";
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
}
