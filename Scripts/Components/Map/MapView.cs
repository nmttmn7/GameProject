using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Godot;
using TheLiquidFire.AspectContainer;

public partial class MapView : Node, IAspect {

#region Container Setup
	public IContainer container { 
		get {
			if (_container == null) {
				_container = new TheLiquidFire.AspectContainer.Container();
				_container.AddAspect (this);
				_container.AddAspect<ActionSystem> ();
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
	
	}

    public override void _ExitTree()
    {
		container.Destroy();
    }
    public override void _Ready()
	{
		if(FileFactory.Contains(DataManager.mapPath,"map")){
		LoadMap();
		SaveFactory.SaveCurrentScene("res://Scenes/MapScene.tscn");
		}else{
		GenerateMap();
		SaveFactory.SavePlayerMap(this);
		SaveFactory.SaveCurrentScene("res://Scenes/MapScene.tscn");
		}
		
		

	}


	private void LoadMap(){
		int x = 1;
		var file =  Godot.FileAccess.Open(DataManager.mapPath,Godot.FileAccess.ModeFlags.Read);
		var fileText = file.GetAsText();
		var contents = MiniJSON.Json.Deserialize (fileText) as Dictionary<string, object>;
		file.Close();
		var array = (List<object>)contents ["map"];
	//	var array = (List<object>)contents ["map"];
		for(int i = 0; i < MapMaxHeightY; i++){
		string text = "";
		text += "\"depth\": " + "\"" + i + "\""; 
		var rowCount = Regex.Matches(fileText,text).Count;
	
		jaggedMap[i] = new Node[rowCount];
		
		for(int b = 0; b < jaggedMap[i].Length; b++){
			
			var nodeData = (Dictionary<string, object>)array.ElementAt(x);
			
			int depth = System.Convert.ToInt32(nodeData["depth"]);
			var positionText = (string)nodeData["mapPosition"];

			string mapX = positionText.Substring(1,positionText.IndexOf(",")-1);
			positionText = positionText.Substring(positionText.IndexOf(",") + 1);
			string mapY = positionText.Substring(0,positionText.Length-1);
			
			Vector2 mapPosition;
			mapPosition.X = float.Parse(mapX);
			mapPosition.Y = float.Parse(mapY);
			
			var path = (string)nodeData["path"];
			string id = (string)nodeData["mapID"];
			//jaggedMap[i][b] = LoadDestination(depth,b,mapPosition,path);
			jaggedMap[i][b] = GenerateDestination(mapPosition,depth,b,id);
			x++;
		//ADD Nodes
		}
		}
		
		LoadWorldData(array);
		LoadCurrentNode(array);
		//AFTER ADDING NODES GENERATE THE CONNECTS
		//PROBABLY BASED ON X AND Y COORDS OF JAGGED MAP DO NOT DO MAPPOSITION IN WORLD. 
		//GENERATING MAP POSITION U WILL HAVE TO TRAVEL MAP MULTIPLE TIMES TO FIND THE CONNECTED WHEN U COULD JUST SET CONNECTION EQUAL LIKE jagged[x][x] = jagged[c][c]
		LoadConnections(array);
	}
	private void LoadWorldData(List<object> array){
	var nodeData = (Dictionary<string, object>)array.ElementAt(0);
	mapWorld = (string)nodeData["mapWorld"];
	playerDepth = System.Convert.ToInt32(nodeData["playerDepth"]);
	}
    private void LoadCurrentNode(List<object> array)
    {
    var nodeData = (Dictionary<string, object>)array.ElementAt(0);
	var currentString = (string)nodeData["currentMapNodeX"];
	if(currentString == "null")
		return;
	
	int currentMapNodeX = System.Convert.ToInt32(nodeData["currentMapNodeX"]);
	int depth = System.Convert.ToInt32(nodeData["currentMapNodeDepth"]);
	currentNode = jaggedMap[depth][currentMapNodeX].GetChild<MapNode>(0);
	currentNode.animatedSprite2D.Frame = 5;
    }

    private void LoadConnections(List<object> array)
    {
		int x = 1;
		for(int i = 0; i < jaggedMap.Length; i++){
		
		for(int b = 0; b < jaggedMap[i].Length; b++){
		var currentMapNode = jaggedMap[i][b].GetChild<MapNode>(0);
		var nodeData = (Dictionary<string, object>)array.ElementAt(x);
		var connectedData = (Dictionary<string, object>)nodeData["connectedNodes"];

		int o = 0;
		while(true){
		if(!connectedData.ContainsKey("connectedX" + o.ToString()))
		break;
		var jaggedX = System.Convert.ToInt32(connectedData["connectedX"+ o.ToString()]);
	
		//currentMapNode.mapNodes.Add(jaggedMap[i+1][jaggedX].GetChild<MapNode>(0));
		GeneratePath(jaggedMap[i][b], jaggedMap[i+1][jaggedX]);
		o++;
		}
		x++;
		}
		
		}
	}
	
	public override void _Process(double delta)
	{
		actionSystem.Update ();

		if(Input.IsActionJustPressed("ScrollUp")){
			node.Position += new Vector2(0,20f);
		}
		if(Input.IsActionJustPressed("ScrollDown")){
			node.Position += new Vector2(0,-20f);
		}
	}

	#endregion


public string mapWorld;
public int playerDepth = 1;


	public MapNode currentNode = null;
	[Export] PackedScene mapDestinationConstruct;
	[Export] Node2D node;
	[Export] Node Lines;
	[Export] Node Destinations;
	
	
	
	public Node[][] jaggedMap = new Node[MapMaxHeightY][];

	const int MapMaxHeightY = 7;
	public float paddingY = 200f;
	public float restrictionX = 400f;

	private void GenerateMap(){

			GenerateWorld();

		for(int i = 0; i < MapMaxHeightY; i++){
			GenerateRow(i);	
		}
		
		GenerateConnections();
		
	}

	private void GenerateWorld(){
		var rand = RNGFactory.RandiRange(1,1);

		switch(rand){
			case 1:
			mapWorld = "World1";
			return;
			case 2:
			mapWorld = "World2";
			return;
			case 3:
			mapWorld = "World3";
			return;
		}
		
	}

	  private void GenerateConnections(){

		for(int i = 0; i < MapMaxHeightY - 1; i++){
			var bottom = jaggedMap[i];
			var top = jaggedMap[i + 1];
			
			for(int b = 0; b < bottom.Length; b++){
				
				var rand = RNGFactory.RandiRange(SearchForLastConnection(top),top.Length - 1);
				
				if(b == bottom.Length - 1)
					rand = top.Length - 1;
					
				for(int r = rand; r > -1; r--){
					var connected = top[r].GetChild<MapNode>(0).connected;
					if(connected != true || rand == r){
					GeneratePath(bottom[b],top[r]);
					top[r].GetChild<MapNode>(0).connected = true;
				
					}
					
				}

				


			}

		}
	}

		 private void GeneratePath(Node start, Node end){

		start.GetChild<MapNode>(0).mapNodes.Add(end.GetChild<MapNode>(0));
		Line2D instance = new Line2D();
		Lines.AddChild(instance);
		
		
			var startPos = (Node2D)start;
			var endPos = (Node2D)end;
			
		
		instance.AddPoint(startPos.Position);
		instance.AddPoint(endPos.Position);
		instance.DefaultColor = new Color(0, 0, 0, 1);

	}

	private int SearchForLastConnection(Node[] top)
    {
		for(int i = 1; i < top.Length; i++){
				if(!top[i].GetChild<MapNode>(0).connected){
					return i - 1;
				}
		}
		return top.Length - 1;
	}

	int minIndexs = 1;
	int maxIndexes = 4;
	float xRange = 400;
	public float paddingX = 50f;

	private void GenerateRow(int height){
		int indexes = RNGFactory.RandiRange(minIndexs,maxIndexes);
		
		
		var width = xRange * 2;
		var leftX = -xRange;
		var section = width / indexes;
		var rightX = leftX + section;
		jaggedMap[height] = new Node[indexes];
		
		for(int i = 0; i < indexes; i++){
		float randF = RNGFactory.RandfRange(leftX + paddingX,rightX - paddingX);
		var mapPosition = node.Position;
		mapPosition.X = randF;
		mapPosition.Y -= height * 200;
		jaggedMap[height][i] =	GenerateDestination(mapPosition, height, i, GenerateMapID(height));
		leftX += section;
		rightX += section;
	}
	}

	int encounterCount = 0;
	int storeCount = 0;
	int eliteCount = 0;
	

	private string GenerateMapID(int depth){
		
		if(depth == 0){
		return "Regular";
		}else if(depth == MapMaxHeightY - 1){
		return "Boss";
		}else{
		
		
		List<string> list = new();
		list.Add("Regular");
		if(eliteCount <= 1){
			list.Add("Elite");
		}
		if(encounterCount <= 1){
			list.Add("Encounter");
		}
		if(storeCount <= 1){
			list.Add("Store");
		}
		
		
		 
		return RNGFactory.RandListInteger(list);
		
		
		}

	}
	public static string GetCurrentMapNodeData(string reference){
		var file =  Godot.FileAccess.Open(DataManager.mapPath,Godot.FileAccess.ModeFlags.Read);
		var fileText = file.GetAsText();
		var contents = MiniJSON.Json.Deserialize (fileText) as Dictionary<string, object>;
		file.Close();
		var array = (List<object>)contents ["map"];


		var nodeData = (Dictionary<string, object>)array.ElementAt(0);
		var data = (string)nodeData[reference];
		
		return data;
		

	}
	private Node  GenerateDestination(Vector2 position, int depth, int jaggedX, string mapNodeID)
    {
		
		
        var instance = mapDestinationConstruct.Instantiate();
		Destinations.AddChild(instance);
		Node2D node2D = (Node2D)instance;
		node2D.Position = position; 

		var mapNode = instance.GetChild<MapNode>(0);
		mapNode.depth = depth;
		mapNode.mapPosition = position;
		mapNode.jaggedX = jaggedX;
		mapNode.jaggedY = depth;

		

		if(mapNodeID == "Boss"){
			//BOSS NODE
			mapNode.mapID = "Boss";
			mapNode.path = "res://Scenes/Main.tscn";
			mapNode.animatedSprite2D.Frame = 2;
		//	mapNode.sprite2D.Texture = (Texture2D)GD.Load("uid://sy2j2qo0j8wn");
		}else if(mapNodeID == "Regular"){
			//REGULAR Node
			mapNode.mapID = "Regular";
			mapNode.path = "res://Scenes/Main.tscn";
			mapNode.animatedSprite2D.Frame = 0;
		//	mapNode.mapButton.Icon = (Texture2D)GD.Load("res://Sprites/MapIcons/Skull.jpg");
		}else if(mapNodeID == "Elite"){
			//Elite Node
			
			mapNode.mapID = "Elite";
			eliteCount ++;
			mapNode.path = "res://Scenes/Main.tscn";
			mapNode.animatedSprite2D.Frame = 1;
		//	Material material = mapNode.animatedSprite2D.Material;
		//	ShaderMaterial shaderMaterial = ((ShaderMaterial)material);
			
		//	shaderMaterial.SetShaderParameter("shift_color", new Vector4(1, 0, 0, 1));
		//	mapNode.sprite2D.Texture = (Texture2D)GD.Load("uid://cn63o5dvu3s08");
		}else if(mapNodeID == "Encounter"){
			//Encounter Node
			mapNode.mapID = "Encounter";
			encounterCount++;
			mapNode.path = "res://Scenes/EncounterScene.tscn";
			mapNode.animatedSprite2D.Frame = 6;
		//	mapNode.sprite2D.Texture = (Texture2D)GD.Load("uid://ctwwocup1nony");
		}else if(mapNodeID == "Store"){
			//Store Node
			mapNode.mapID = "Store";
			storeCount++;
			mapNode.path = "res://Scenes/StoreScene.tscn";
			mapNode.animatedSprite2D.Frame = 3;
		//	mapNode.sprite2D.Texture = (Texture2D)GD.Load("uid://dacb407fvj3kr");
		}else if(mapNodeID == "Constructor"){
			//Store Node
			GD.Print("CONTRUCT");
			mapNode.mapID = "Constructor";
			storeCount++;
			mapNode.path = "res://Scenes/CardConstructorScene.tscn";
			mapNode.animatedSprite2D.Frame = 7;
		//	mapNode.sprite2D.Texture = (Texture2D)GD.Load("uid://dacb407fvj3kr");
		}

		return instance;
    }

	
}