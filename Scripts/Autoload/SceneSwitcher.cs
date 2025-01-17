using Godot;
using System;
using System.Collections.Generic;
using GodotPlugins;
public partial class SceneSwitcher : Node
{
		Node currentScene = null;

		public static SceneSwitcher node;

		public List<string> dispositions= new List<string>();

		public static Node AudioManager;
		public string wow ="";
	public override void _Ready()
	{
		node = this;
		var root = GetTree().Root;
		currentScene = root.GetChild(root.GetChildCount() - 1);
		AudioManager = root.GetNode("AudioManager");
		
		ColorRect colorRect = new();
		
	}

	
	
	
	public void SwitchScene(string path){
	
		var pointsDict = new Godot.Collections.Dictionary
{
    { "speed", 4 },
	 { "wait_time", 0 }
  
};
 GetTree().Root.GetChild(2).Call("change_scene", path, pointsDict);
	
	}

	public void _deferred_switch_scene(string path){
		currentScene.Free();
		PackedScene s = (PackedScene)GD.Load(path);
		currentScene = s.Instantiate();
		GetTree().Root.AddChild(currentScene);
		GetTree().CurrentScene =  currentScene;
	}

	public void CreateTransition(int ID){
		switch(ID)
		{
			case 0:
			
			GetTree().Root.AddChild(new ColorRect());

			return;
		}
	}



}
