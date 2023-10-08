using Godot;
using System;

public partial class Mob : RigidBody2D
{
	private AnimatedSprite2D _animatedSprite2D;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		GetNode<VisibleOnScreenNotifier2D>("VisibleOnScreenNotifier2D").ScreenExited += OnVisibleOnScreenNotifier2DScreenExited;
		
		string[] animationNames = _animatedSprite2D.SpriteFrames.GetAnimationNames();
		string animationName = animationNames[GD.Randi() % animationNames.Length];
		_animatedSprite2D.Play(animationName);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void OnVisibleOnScreenNotifier2DScreenExited()
	{
		QueueFree();
	}
}
