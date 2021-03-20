using Godot;
using System;
using System.Collections;

public class Singleton : Node
{
	// Global variables
	public int LevelNumber { get; set; }
	public int PlayerMaxHealth
	{
		get
		{
			return PlayerEvolution > 0 ? 150 : 100;
		}
	}
	public float PlayerTopSpeed
	{
		get
		{
			return PlayerEvolution > 1 ? 250f : 200f;
		}
	}

	public float PlayerAcceleration
	{
		get
		{
			return PlayerEvolution > 1 ? 40f : 20f;
		}
	}

	public float PlayerFireRate
	{
		get
		{
			return PlayerEvolution > 2 ? 0.1f : 0.25f;
		}
	}

	public float BulletScale
	{
		get
		{
			return PlayerEvolution > 2 ? 0.8f : 0.5f;
		}
	}

	public int PlayerEvolution { get; set; }

	public enum FiringMode
	{
		MONO,
		BI,
		QUAD,
	}

	public FiringMode PlayerFiringMode
	{
		get
		{
			if (PlayerEvolution <= 4)
			{
				return FiringMode.MONO;
			}
			else if (PlayerEvolution > 4 && PlayerEvolution < 7)
			{
				return FiringMode.BI;
			}
			else
			{
				return FiringMode.QUAD;

			}
		}
	}

	public bool SpeedrunActive { get; set; }

	// Children nodes
	private CanvasLayer _hud;
	private AudioStreamPlayer _audio;

	// Local variables
	private ArrayList _actions;

	private float _speedrunTime;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_hud = GetNode<CanvasLayer>("HUD");
		_audio = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
		PlayerEvolution = 0;
		LevelNumber = 1;
		SpeedrunActive = false;
		_speedrunTime = 0f;

		_audio.Play();

		_actions = new ArrayList(){
			"up",
			"down",
			"left",
			"right",
			"fire",
		};
	}

	public void SetHUDVisibility(bool visible)
	{
		var control = _hud.GetNode<Control>("Control");
		control.Visible = visible;
	}

	public void UpdateHealth(int currentHealth)
	{
		var health = _hud.GetNode<TextureRect>("Control/Health");
		((ShaderMaterial)health.Material).SetShaderParam("health_percentage", ((float)currentHealth / (float)PlayerMaxHealth));
	}

	public void UpdateMobCount(int mobCount)
	{
		var mobLabel = _hud.GetNode<Label>("Control/Counter/mob");
		mobLabel.Text = $"Mutants: {mobCount}";
	}
	public void ScrambleControls()
	{
		// Add rotate inputs to the scramble if the player has unlocked it.
		if (PlayerEvolution > 3 && !_actions.Contains("rotate_left") && !_actions.Contains("rotate_right"))
		{
			_actions.Add("rotate_left");
			_actions.Add("rotate_right");
		}

		var activeInputs = new ArrayList();
		foreach (string action in _actions)
		{
			activeInputs.Add(InputMap.GetActionList(action)[0]);
			InputMap.ActionEraseEvents(action);
		}

		// Randomly assign inputs to said events.
		var random = new Random();
		foreach (string action in _actions)
		{
			int index = random.Next(activeInputs.Count);
			InputMap.ActionAddEvent(action, (InputEvent)activeInputs[index]);
			activeInputs.RemoveAt(index);
		}

	}

	public void UpdateControls()
	{
		if (PlayerEvolution > 3)
		{
			_hud.GetNode<Label>("Control/Inputs/rotate_left").Visible = true;
			_hud.GetNode<Label>("Control/Inputs/rotate_right").Visible = true;
		}

		var inputs = _hud.GetNode<VBoxContainer>("Control/Inputs");
		foreach (Label label in inputs.GetChildren())
		{
			label.Text = $"{label.Name}: {((InputEvent)InputMap.GetActionList(label.Name)[0]).AsText()}";
		}
	}

	private void _updateTime()
	{
		var timeLabel = _hud.GetNode<Label>("Control/Counter/time");
		var minute = (int)_speedrunTime / 60;
		timeLabel.Text = $"Time: {minute}:{Math.Round(_speedrunTime % 60, 2)}";
	}

	public void UpdateLevel()
	{
		var levelLabel = _hud.GetNode<Label>("Control/Counter/level");
		levelLabel.Text = $"Level {LevelNumber}";
	}
	//  // Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{
		if (SpeedrunActive)
		{
			_speedrunTime += delta;
			_updateTime();
		}
	}
}
