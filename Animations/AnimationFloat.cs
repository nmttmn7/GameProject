using Godot;
using System;
using System.Numerics;

public partial class AnimationFloat : Node
{

	public float time = 0;
	public float time_multiplier = 2;
	public Node2D node2D; 
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		node2D = this.GetParent<Node2D>();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		time += (float)delta;
		
		node2D.Position += new Godot.Vector2(0,MathF.Cos(time * time_multiplier));
		GD.Print("MATHF " + MathF.Sin(time * time_multiplier));
	}
}
