using Godot;
using System;

public partial class Player : Area2D
{
	[Signal]
	public delegate void HitEventHandler(int hp);
	
	[Signal]
	public delegate void DieEventHandler();
	
	[Export]
	public float Speed { get; set; } = 400;

	private Vector2 _screenSize;
	private AnimatedSprite2D _animatedSprite2D;
	private CollisionShape2D _collisionShape2D;
	private int _hp;
    
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_collisionShape2D = GetNode<CollisionShape2D>("CollisionShape2D");
		_screenSize = GetViewportRect().Size;
		BodyEntered += OnBodyEntered;
		Hide();
		SetProcess(false);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// 监听输入的方向
		Vector2 dir = ListenerInputDirection();
		if (dir.Length() > 0)
		{
			// 计算速度向量
			Vector2 velocity = dir.Normalized() * Speed;
			// 计算上一帧到这一帧的速度向量
			Vector2 frameVelocity = velocity * (float)delta;
			// 计算目标位置
			Vector2 targetPosition = Position + frameVelocity;
			// 设置位置
			SetPosition(targetPosition);

			// 根据方向设置动画播放数据
			SetAnimationInfo(dir);
			_animatedSprite2D.Play();
		}
		else
		{
			_animatedSprite2D.Stop();
		}
	}

	public void Start(Vector2 position, int hp)
	{
		_hp = hp;
		SetPosition(position);
		Show();
		_collisionShape2D.Disabled = false;
		SetProcess(true);
	}

	private Vector2 ListenerInputDirection()
	{
		Vector2 dir = Vector2.Zero;
		if (Input.IsActionPressed(KeyMap.MOVE_UP))
		{
			dir.Y = -1;
		}
		if (Input.IsActionPressed(KeyMap.MOVE_DOWN))
		{
			dir.Y = 1;
		}
		if (Input.IsActionPressed(KeyMap.MOVE_LEFT))
		{
			dir.X = -1;
		}
		if (Input.IsActionPressed(KeyMap.MOVE_RIGHT))
		{
			dir.X = 1;
		}

		return dir;
	}

	private void SetPosition(Vector2 targetPosition)
	{
		// 限制目标位置在屏幕范围内
		targetPosition.X = Mathf.Clamp(targetPosition.X, 0, _screenSize.X);
		targetPosition.Y = Mathf.Clamp(targetPosition.Y, 0, _screenSize.Y);
		
		Position = targetPosition;
	}

	private void SetAnimationInfo(Vector2 dir)
	{
		if (dir.X != 0)
		{
			_animatedSprite2D.Animation = "walk";
			_animatedSprite2D.FlipH = dir.X < 0;
		}
		else if (dir.Y != 0)
		{
			_animatedSprite2D.Animation = "up";
			_animatedSprite2D.FlipV = dir.Y > 0;
		}
	}

	private void OnBodyEntered(Node2D body)
	{
		_hp--;
		if (_hp <= 0)
		{
			PlayerDie();
		}
		else
		{
			PlayerHit();
		}
	}

	private void PlayerDie()
	{
		Hide();
		_collisionShape2D.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
		SetProcess(false);
		EmitSignal(SignalName.Die);
	}

	private void PlayerHit()
	{
		EmitSignal(SignalName.Hit, _hp);
	}
}
