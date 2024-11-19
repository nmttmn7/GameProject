using Godot;
using Godot.Collections;
using System;

public partial class DataManager : Node
{

	[Export] public Dictionary<string, CardData> _cardCompendium = new();
	// Called when the node enters the scene tree for the first time.

	Node currentScene = null;

	public static DataManager node;

	public Array<Dispositions> dispositions= new Array<Dispositions>();
	public override void _Ready()
	{	
		node = this;
		OO();
	}

	

	private void TT(){
		Dictionary<string,CardData> cards = _cardCompendium.Duplicate();
		
	
	}
	private void OO(){

		var folder = "res://Data/CardCompedium/Units/";

		var dir = DirAccess.Open(folder);
		var allDir = dir.GetDirectories();

		foreach(var d in allDir){
			
	var newDir = DirAccess.Open(folder + d);
	var allFiles = newDir.GetFiles();

	foreach(var loadFile in allFiles){
	
	var temp = ResourceLoader.Load<Card>(folder + d + "/" + loadFile);
	
	}

	}
	}
}
