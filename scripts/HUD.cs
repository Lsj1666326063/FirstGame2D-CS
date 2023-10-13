using Godot;
using System;

public partial class HUD : CanvasLayer
{
	[Signal]
	public delegate void StartGameEventHandler();

	private Timer _msgTimer;
	private Label _msgLabel;
	private Label _hpLabel;
	private Label _scoreLabel;
	private Button _startBtn;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_msgTimer = GetNode<Timer>("MsgTimer");
		_msgTimer.Timeout += OnMsgTimerTimeOut;
		_msgLabel = GetNode<Label>("MsgLabel");
		_hpLabel = GetNode<Label>("HpLabel");
		_scoreLabel = GetNode<Label>("ScoreLabel");
		_startBtn = GetNode<Button>("StartButton");
		_startBtn.Pressed += OnStartBtnPressed;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void ShowMsg(string msg)
	{
		_msgLabel.Text = msg;
		_msgLabel.Show();
		_msgTimer.Start();
	}

	public async void ShowGameOver()
	{
		ShowMsg("Game Over");

		await ToSignal(_msgTimer, Timer.SignalName.Timeout);
		
		_msgLabel.Text = "Dodge the Creeps!";
		_msgLabel.Show();

		await ToSignal(GetTree().CreateTimer(1.0), SceneTreeTimer.SignalName.Timeout);
		
		_startBtn.Show();
	}

	public void UpdateHp(int hp)
	{
		_hpLabel.Text = hp.ToString();
	}

	public void UpdateScore(int score)
	{
		_scoreLabel.Text = score.ToString();
	}

	private void OnMsgTimerTimeOut()
	{
		_msgLabel.Hide();
	}

	private void OnStartBtnPressed()
	{
		_startBtn.Hide();
		EmitSignal(SignalName.StartGame);
	}
}
