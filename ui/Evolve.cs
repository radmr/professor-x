using Godot;
using System;

public class Evolve : Control
{
    // Children nodes
    private Label _mainText;
    private Button _reject;
    private Button _accept;
    private Button _continue;

    // Local properties
    private Godot.Collections.Dictionary _nextEvolution;
    private Godot.Collections.Dictionary _data;

    // Signals
    [Signal] public delegate void MutateControls();


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {

        _mainText = GetNode<Label>("Container/MainText");
        _reject = GetNode<Button>("Container/ButtonContainer/NoButton");
        _accept = GetNode<Button>("Container/ButtonContainer/YesButton");
        _continue = GetNode<Button>("Container/ButtonContainer/ContinueButton");

        var file = new Godot.File();

        file.Open("res://global/data.json", Godot.File.ModeFlags.Read);
        _data = JSON.Parse(file.GetAsText()).Result as Godot.Collections.Dictionary;

        file.Close();
        var prompts = _data["prompts"] as Godot.Collections.Dictionary;

        _mainText.Text = (string)prompts["mainText"];
        _reject.Text = (string)prompts["reject"];
        _accept.Text = (string)prompts["accept"];


        var singleton = GetNode<Singleton>("/root/Singleton");
        Connect("MutateControls", singleton, "ScrambleControls");
        singleton.SpeedrunActive = false;

        var evolutions = _data["evolutions"] as Godot.Collections.Array;
        if (singleton.PlayerEvolution < evolutions.Count)
        {
            _nextEvolution = evolutions[singleton.PlayerEvolution] as Godot.Collections.Dictionary;
            var mutateProbability = (float)_nextEvolution["mutateProbability"] * 100;
            _accept.Text = _accept.Text + $" ({mutateProbability}%)";
        }
        else
        {
            _accept.Disabled = true;
        }
    }


    public void _on_YesButton_pressed()
    {
        _reject.Visible = false;
        _accept.Visible = false;
        _continue.Visible = true;

        var resultText = _nextEvolution["name"].ToString() + "\n" + _nextEvolution["description"].ToString();
        var random = new Random();

        var mutateProbability = (float)_nextEvolution["mutateProbability"];
        if (mutateProbability > random.NextDouble())
        {
            resultText += "\nBut the radiation in the water has scrambled your nervous system.\nControls randomised.";
            EmitSignal("MutateControls");
        }
        GetNode<Singleton>("/root/Singleton").PlayerEvolution++;
        GetNode<Singleton>("/root/Singleton").UpdateControls();


        _mainText.Text = resultText;
    }

    public void _on_NoButton_pressed()
    {
        _reject.Visible = false;
        _accept.Visible = false;
        _continue.Visible = true;

        var prompts = _data["prompts"] as Godot.Collections.Dictionary;
        _mainText.Text = prompts["rejectResult"].ToString();


    }

    public void _on_ContinueButton_pressed()
    {
        GetNode<Singleton>("/root/Singleton").LevelNumber++;
        var level = GetNode<Singleton>("/root/Singleton").LevelNumber;
        GetTree().ChangeScene($"res://levels/Level{level}.tscn");
    }
    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
}
