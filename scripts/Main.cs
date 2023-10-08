using Godot;
using System;

public partial class Main : Node
{
	[Export]
	public PackedScene PlayerPrefab;
	
	[Export]
	public PackedScene MobPrefab;

	private Timer _startTimer;
	private Timer _mobTimer;
	private Timer _scoreTimer;
	private Vector2 _playerStartPos;
	private PathFollow2D _mobSpawnLocation;
	
	private int _score;

	private Player _player;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_startTimer = GetNode<Timer>("StartTimer");
		_startTimer.Timeout += OnStartTimerTimeout;
		_mobTimer = GetNode<Timer>("MobTimer");
		_mobTimer.Timeout += OnMobTimerTimeout;
		_scoreTimer = GetNode<Timer>("ScoreTimer");
		_scoreTimer.Timeout += OnScoreTimerTimeout;
		_playerStartPos = GetNode<Marker2D>("PlayerStartPosition").Position;
		_mobSpawnLocation = GetNode<PathFollow2D>("MobPath/MobSpawnLocation");

		_player = PlayerPrefab.Instantiate<Player>();
		AddChild(_player);
		
		NewGame();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void NewGame()
	{
		_score = 0;
		_player.Start(_playerStartPos);
		
		_startTimer.Start();
	}

	private void GameOver()
	{
		_mobTimer.Stop();
		_scoreTimer.Stop();
	}

	private void OnStartTimerTimeout()
	{
		_mobTimer.Start();
		_scoreTimer.Start();
	}

	private void OnMobTimerTimeout()
	{
		// 实例化怪
		Mob mob = MobPrefab.Instantiate<Mob>();
		
		// 设置怪的坐标（item1） 旋转（item2）
		var randomMobInfo = RandomMobPosAndDir();
		mob.Position = randomMobInfo.Item1;
		mob.Rotation = randomMobInfo.Item2;

		// 没看懂这个速度
		var velocity = new Vector2((float)GD.RandRange(150.0, 250.0), 0);
		mob.LinearVelocity = velocity.Rotated(randomMobInfo.Item2);

		// Spawn the mob by adding it to the Main scene.
		AddChild(mob);
	}

	private void OnScoreTimerTimeout()
	{
		_score++;
	}

	private (Vector2, float) RandomMobPosAndDir()
	{
		// Choose a random location on Path2D.
		// 在路径中随机一个位置
		_mobSpawnLocation.ProgressRatio = GD.Randf();
		Vector2 position = _mobSpawnLocation.Position;

		// Set the mob's direction perpendicular to the path direction.
		// _mobSpawnLocation的方向是沿路径旋转的，（π/2表示弧度，就是角度90度），
		// Rotation 加上 90度就是垂直于路径方向了 [根据上面小括号的注释进行猜测的]
		float direction = _mobSpawnLocation.Rotation + Mathf.Pi / 2;
		// Add some randomness to the direction.
		// 在当前朝向的 ±45度 范围内随机一个角度相加
		direction += (float)GD.RandRange(-Mathf.Pi / 4, Mathf.Pi / 4);
		
		return (position, direction);
	}
}
