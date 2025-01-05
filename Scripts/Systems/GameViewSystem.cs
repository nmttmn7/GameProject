using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Godot;
using TheLiquidFire.AspectContainer;

public partial class GameViewSystem : Node, IAspect {

	bool loadDeck = true;
	UIView UIView;

	public IContainer container { 
		get {
			if (_container == null) {
				_container = GameFactory.Create ();
				_container.AddAspect (this);
			}
			return _container;
		}
		set {
			_container = value;
		}
	}
	IContainer _container;

	ActionSystem actionSystem;



	 public override void _EnterTree()
    {
		container.Awake ();
		actionSystem = container.GetAspect<ActionSystem> ();


		SaveFactory.SaveCurrentScene("res://Scenes/Main.tscn");


		Temp_SetupSinglePlayer ();
		SetupOpposer();

		var system = container.GetAspect<MatchSystem> ();
		system.ChangeTurn ();

	}

 	private void Temp_SetupSinglePlayer()
    {
		var match = container.GetMatch ();
		match.players [0].mode = ControlModes.Local;
		Player p = match.players[0];

		List<Card> deck = new();

		if(loadDeck && FileFactory.Contains(DataManager.playerdeckPath, "deck")){
			//GD.Print("LOADSSS");
			deck = DeckFactory.CreateDeck(DataManager.playerdeckPath, p.index);

		}else{

			if(SceneSwitcher.node.dispositions[0] == "D1")
			deck = DeckFactory.CreateDeck ("res://Data/PackCollection/Player/D1StarterPack.txt", p.index);

			if(SceneSwitcher.node.dispositions[0] == "D2")
			deck = DeckFactory.CreateDeck ("res://Data/PackCollection/Player/D2StarterPack.txt", p.index);

			if(SceneSwitcher.node.dispositions[0] == "D3")
			deck = DeckFactory.CreateDeck ("res://Data/PackCollection/Player/D3StarterPack.txt", p.index);
		}

		var statusSystem = container.GetAspect<StatusSystem>();
		foreach(Card card in deck) 
			statusSystem.InitializeCard(card, DeckFactory.Cards[card.id]);

		p [Zones.Deck].AddRange (deck);
		
		
		//	p [Zones.Graveyard].AddRange (deck);
    }

    private void SetupOpposer()
    {
        var match = container.GetMatch ();
		match.players [1].mode = ControlModes.Computer;
		Player p = match.players[1];
		List<Card> deck = new();
		string path = "";
		if(!FileFactory.Contains(DataManager.enemydeckPath, ".txt")){

		path =  "res://Data/PackCollection/Enemy/";
		var data = mapWorld();

		
		path += data[0] +"/"+ data[1] + "/";

			var dir = DirAccess.Open(path);
			dir.ListDirBegin();
			string[] allFiles = dir.GetFiles();
			dir.ListDirEnd();
			var i = RNGFactory.RandiRange(0,allFiles.Length - 1);

		path += i +".txt";

	
	//	GD.Print("PATH= " + path);
	//	GD.Print("files= " + allFiles.Length);
		
		

		var loadFile = Godot.FileAccess.Open(DataManager.enemydeckPath, Godot.FileAccess.ModeFlags.Write);
		loadFile.StoreString(path);
		loadFile.Close();

		}else{

		var loadFile = Godot.FileAccess.Open(DataManager.enemydeckPath, Godot.FileAccess.ModeFlags.Read);
		var str = loadFile.GetAsText();
		loadFile.Close();
		path = str;

		}
		
		deck = DeckFactory.CreateDeck (path, p.index);
		
	//	var statusSystem = container.GetAspect<StatusSystem>();
	//	foreach(Card card in deck) 
	//		statusSystem.InitializeCard(card, DeckFactory.Cards[card.id]);

		
		p [Zones.Deck].AddRange (deck);

		var file = Godot.FileAccess.Open(path, Godot.FileAccess.ModeFlags.Read);
		var fileText = file.GetAsText();
		var contents = MiniJSON.Json.Deserialize (fileText) as Dictionary<string, object>;
		file.Close();

		var array = (List<object>)contents ["draw"];
		var result = new List<int> ();

		foreach (object item in array) {
			var n = Int32.Parse((string)item);
			result.Add(n);
		}

		p.drawList = result;

    }

	public List<string> mapWorld(){
		List<string> data = new();
		var file =  Godot.FileAccess.Open(DataManager.mapPath,Godot.FileAccess.ModeFlags.Read);
		var fileText = file.GetAsText();
		var contents = MiniJSON.Json.Deserialize (fileText) as Dictionary<string, object>;
		file.Close();
		var array = (List<object>)contents ["map"];
		var nodeData = (Dictionary<string, object>)array.ElementAt(0);
		data.Add((string)nodeData["mapWorld"]);
		data.Add((string)nodeData["currentMapNodeID"]);
		return data;

	}
   


    public override void _ExitTree()
    {
		container.Destroy();
    }
    public override void _Ready()
	{	
		container.ChangeState<PlayerIdleState> ();
		UIView = GetTree().Root.GetNode("Main").GetNode("GameViewSystem").GetNode<UIView>("UIView");
		UIView.StartUI(container);
	}

	public override void _Process(double delta)
	{
		actionSystem.Update ();
	}


}