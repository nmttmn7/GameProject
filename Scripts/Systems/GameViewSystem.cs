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
	public static string deckFilePath = "res://UserData/Cards/Player/Deck.txt";
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

		if(loadDeck && FileFactory.Contains("res://UserData/Cards/Player/Deck.txt", "deck")){
			//GD.Print("LOADSSS");
			deck = DeckFactory.CreateDeck("res://UserData/Cards/Player/Deck.txt", p.index);

		}else{

			if(SceneSwitcher.node.dispositions[0] == "d1")
			deck = DeckFactory.CreateDeck ("res://UserData/Cards/Player/FirstStarterPack.txt", p.index);

			if(SceneSwitcher.node.dispositions[0] == "d2")
			deck = DeckFactory.CreateDeck ("res://UserData/Cards/Player/SecondStarterPack.txt", p.index);

			if(SceneSwitcher.node.dispositions[0] == "d3")
			deck = DeckFactory.CreateDeck ("res://UserData/Cards/Player/ThirdStarterPack.txt", p.index);
		}

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
		if(!FileFactory.Contains("res://UserData/Cards/Player/Deck.txt", ".txt")){

		path =  "res://UserData/Cards/Enemy/";
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
		
		

		var loadFile = Godot.FileAccess.Open("res://UserData/Cards/Enemy/Deck.txt", Godot.FileAccess.ModeFlags.Write);
		loadFile.StoreString(path);
		loadFile.Close();

		}else{

		var loadFile = Godot.FileAccess.Open("res://UserData/Cards/Enemy/Deck.txt", Godot.FileAccess.ModeFlags.Read);
		var str = loadFile.GetAsText();
		loadFile.Close();
		path = str;

		}
		
		deck = DeckFactory.CreateDeck (path, p.index);
		


		
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
		var file =  Godot.FileAccess.Open(MapView.mapPath,Godot.FileAccess.ModeFlags.Read);
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
/*
	void Temp_SetupSinglePlayer() {
		var match = container.GetMatch ();
		match.players [0].mode = ControlModes.Local;
		Player p = match.players[0];


			List<Card> deck = new();
			
			if(FileFactory.IsEmpty(PathFactory.playerDeckPath)){

			var file =  Godot.FileAccess.Open("res://UserData/Cards/Player/CollectionSelection.txt",Godot.FileAccess.ModeFlags.Read);
			var fileText = file.GetAsText();
			file.Close();


			if(fileText.Equals("RED"))
			deck = DeckFactory.CreateDeck ("res://UserData/Cards/Player/DeckPacks/RedPack.txt", p.index);

			if(fileText.Equals("GREEN"))
			deck = DeckFactory.CreateDeck ("res://UserData/Cards/Player/DeckPacks/GreenPack.txt", p.index);

			}else{
			deck = DeckFactory.LoadDeck(PathFactory.playerDeckPath, p.index);
			}

			p [Zones.Deck].AddRange (deck);

			//GRAVEYARD NEED TO BE ADDED
			if(!FileFactory.IsEmpty(PathFactory.playerGraveyardPath)){
			deck = DeckFactory.LoadDeck (PathFactory.playerGraveyardPath, p.index);
			}
	}

	void SetupOpposer(){
		var match = container.GetMatch ();
		match.players [1].mode = ControlModes.Computer;
		Player p = match.players[1];

	//	if(FileFactory.FindNumberOfStringInstances(PathFactory.enemyDeckPath, "Enemy") <= 0)
	//		CreateEnemyCards();

		GenerateEnemiesBasedOnMapNode();
		
		var deck = DeckFactory.CreateDeck (PathFactory.enemyDeckPath, p.index);
			p [Zones.Deck].AddRange (deck);

			
		RNGFactory.SaveSeed();
	}
	


	void GenerateEnemiesBasedOnMapNode(){

		if(!FileFactory.IsEmpty(PathFactory.enemyDeckPath))
			return;

		var file =  Godot.FileAccess.Open(MapGenerator.mapPath,Godot.FileAccess.ModeFlags.Read);
		var fileText = file.GetAsText();
		var contents = MiniJSON.Json.Deserialize (fileText) as Dictionary<string, object>;
		file.Close();
		var array = (List<object>)contents ["map"];
		var nodeData = (Dictionary<string, object>)array.ElementAt(0);
		int id = System.Convert.ToInt32(nodeData["currentMapNodeID"]); 
		
		
		var worldPath = (string)nodeData["mapWorld"];
		int playerDepth = System.Convert.ToInt32(nodeData["playerDepth"]); 

		
		var type = "Common"; 
		if(id == 0){
			type = "Special";
		}
		if(id == 2){
			type = "Rare";
		}
		
		

			var folder = "res://UserData/Cards/Enemy/DeckPacks/" + worldPath  + "/Lvl" + playerDepth + "/" + type + "/";
			var dir = DirAccess.Open(folder);
			dir.ListDirBegin();
			var allFiles = dir.GetFiles();
			dir.ListDirEnd();
			int i = RNGFactory.RandiRange(0,allFiles.Length - 1);
			var newFile = FileAccess.Open(PathFactory.enemyDeckPath, FileAccess.ModeFlags.Write);
				newFile.StoreString(Godot.FileAccess.Open(folder + allFiles[i],Godot.FileAccess.ModeFlags.Read).GetAsText());
				newFile.Close();
		
		
	}

	void CreateEnemyCards(){
		int MINEnemyCards = 1;
		int MAXEnemyCards = 5;
		
		var cardCom =  Godot.FileAccess.Open(DeckFactory.cardCompendiumPath,Godot.FileAccess.ModeFlags.Read);
		var fileText = cardCom.GetAsText();
		cardCom.Close();
        var enemyListCount = Regex.Matches(fileText, "\"id\":" + " " + "\"" + "Enemy").Count;
		
		


		var amountOfCard = RNGFactory.RandiRange(MINEnemyCards, MAXEnemyCards);

		
		var file = Godot.FileAccess.Open(PathFactory.enemyDeckPath,Godot.FileAccess.ModeFlags.Write);
		file.StoreString("{ \"deck\": [");
		for(int i = 0; i < amountOfCard; i++){
		var c = RNGFactory.RandiRange(1,enemyListCount);
		file.StoreString("\"Enemy"+ c +"\",");
		}
		file.StoreString("\n ]}");
		file.Close();
	


	//	Edit();

	}
	void Edit(){
		var file = Godot.FileAccess.Open(PathFactory.enemyDeckPath,Godot.FileAccess.ModeFlags.Write);
		file.StoreString("{ \"deck\": [");
		file.StoreString("\"Enemy"+ 1 +"\",");
		file.StoreString("\n ]}");
		file.Close();
	
	}
*/
	
}