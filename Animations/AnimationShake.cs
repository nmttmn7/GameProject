using Godot;
using System;
using System.Collections;
using System.Collections.Generic;

public partial class AnimationShake : Node
{
	
	private Control control;
	private Node2D node2D;
	RandomNumberGenerator rng = new();

	Vector2 pivot;
	
	FastNoiseLite noise = new();
	[Export] public float noiseSpeed = 300.0f;
	[Export] public float traumaDecayRate = 2f;

	

	[Export] Vector2 maxDistance = new Vector2(200, 200);
	
	float time = 0f;
	float trauma = 0.0f;

	private int seedX;
	private int seedY;
	
	
	public override void _Ready()
	{
		rng.Randomize();
		
		noise.NoiseType = FastNoiseLite.NoiseTypeEnum.Perlin;

		seedX = (int)rng.Randi();
		seedY = (int)rng.Randi();

		control = GetParentOrNull<Control>();

		if(control == null){
			node2D = GetParentOrNull<Node2D>();
			pivot = node2D.Position;
		}else{
			pivot = control.Position;
		}

		AddTrauma(1);
	
	}

    public override void _Process(double delta)
    {
		
		
		time += (float)delta;

        trauma = (float)Mathf.Max(trauma - delta * traumaDecayRate, 0.0);
		
		var nX =+ pivot.X + maxDistance.X * GetShakeIntensity() * GetNoiseFromSeed(seedX);
		var nY =+ pivot.Y + maxDistance.Y * GetShakeIntensity() * GetNoiseFromSeed(seedY);
	
		if(control == null){
			node2D.Position = new Vector2(nX,nY);
		}else
			control.Position = new Vector2(nX,nY);

		if(trauma == 0)
			this.QueueFree();
		
    }
	
	

	public float GetShakeIntensity(){
		return Mathf.Pow(trauma,2);
	}
    
	public float GetNoiseFromSeed(int seed){
		noise.Seed = seed;
		return noise.GetNoise1D(time * noiseSpeed);
	}
 
	public void AddTrauma(float amount){
		trauma = Mathf.Clamp(trauma + amount, 0, 1);
	} 
}