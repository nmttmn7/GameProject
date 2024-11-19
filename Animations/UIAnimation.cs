using Godot;
using System;
using System.Numerics;

public partial class UIAnimation : Node
{
	[Export]AudioStreamPlayer sfx;
	private float time = 0;
	private float timeX = 0;
	[Export]float amplitude = 3.0f;
	[Export]float frequency = 6.0f;
	[Export]float offset = 0.0f;
	
	[Export]TextureRect card_texture;
	[Export]Node2D node2D;

	Tween tween_expand;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		offsetX = node2D.Position.X;
		offsetY = node2D.Position.Y;
		index = node2D.GetIndex();




		rng.Randomize();
		noise.Seed = (int)rng.Randi();
		noise.NoiseType = FastNoiseLite.NoiseTypeEnum.Perlin;
		//node3D = this.GetChild<Node3D>(0);
		
		pivot = node2D.Position;
		

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
		//RotateZ();
		//MoveTRIG(delta);
		//MoveNOISE(delta);
		

	}

	[Export] Node gpuParent;

	void VFXEffect(){
		var children =	gpuParent.GetChildren();

		foreach (GpuParticles2D child in children)
		child.Restart();
	}
	void SFXEffect(){
		sfx.Play();
	}


	public void Expand(){
	if(tween_expand != null)
		if(tween_expand.IsRunning())
			tween_expand.Kill();
	
	AddTrauma(1);
	VFXEffect();
	SFXEffect();
	
	tween_expand = CreateTween().SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Linear);
	tween_expand.TweenProperty(node2D, "scale", new Godot.Vector2(0.8f, 1.5f), 0.1);
	tween_expand.TweenProperty(node2D, "scale", new Godot.Vector2(1.5f, 0.8f), 0.1);
	tween_expand.TweenProperty(node2D, "scale", new Godot.Vector2(1f, 1f), 0.1);
	

	}

	#region  MoveX
	public float offsetX;
	[Export]float amplitudeX = 3.0f;
	[Export]float frequencyX = 0.75f;

	float offsetY;
	[Export]float amplitudeY = 3.0f;
	[Export]float frequencyY = 0.75f;
	float index = 0.0f;
	[Export] float w = 1.0f;
	
	public void MoveTRIG(double delta){
	
		time += (float)delta;
		Godot.Vector2 pos = new(0,0);

		var s = (Mathf.Sin(time * frequencyY + index) * amplitudeY + offsetY) * w;
		var c = (Mathf.Cos(time * frequencyX + index) * amplitudeX + offsetX) * w;

		
		pos.Y = s;
		pos.X = c;
		
		node2D.Position = pos;

	}

	

	#endregion

	public void RotateZTRIG(){

	var s = Mathf.Sin(time * frequency) * amplitude + offset;
	var c = Mathf.Cos(time * frequency) * amplitude + offset;
	((ShaderMaterial)card_texture.Material).SetShaderParameter("y_rot",s);
	((ShaderMaterial)card_texture.Material).SetShaderParameter("x_rot",c);

	}

//	public void RotateZTWEEN(){
//		Tween tween = CreateTween().Parallel();
//		tween.TweenProperty(card_texture.Material,"shader_parameter/y_rot",58.0f,10.0f);
//		tween.TweenProperty(card_texture.Material,"shader_parameter/x_rot",58.0f,10.0f);
//	}


	FastNoiseLite noise = new();
	[Export] public float noiseSpeed = 800.0f;
	[Export] public float traumaDecayRate = 2f;

	Godot.Vector2 pivot;
	[Export]float maxX = 20f;
	[Export]float maxY = 20f;

	float trauma = 0.0f;

	RandomNumberGenerator rng = new();

	private float noiseTime;
	public void MoveNOISE(double delta)
    {
		if(trauma <= 0 )
			return;
		
		noiseTime += (float)delta;

        trauma = (float)Mathf.Max(trauma - delta * traumaDecayRate, 0.0);

		var nX =+ pivot.X + maxX * GetShakeIntensity() * GetNoiseFromSeed(0);
		var nY =+ pivot.Y + maxY * GetShakeIntensity() * GetNoiseFromSeed(1);
		
	
		node2D.Position = new Godot.Vector2(nX,nY);

		
    }
	
	public void AddTrauma(float amount){
		trauma = Mathf.Clamp(trauma + amount, 0, 1);
	}

	public float GetShakeIntensity(){
		return Mathf.Pow(trauma,2);
	}
    
	public float GetNoiseFromSeed(int seed){
		noise.Seed = seed;
		return noise.GetNoise1D(noiseTime * noiseSpeed);
	}
}
