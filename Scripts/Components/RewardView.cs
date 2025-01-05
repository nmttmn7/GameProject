using System.Collections;
using System.Collections.Generic;

using TheLiquidFire.Notifications;
using TheLiquidFire.AspectContainer;
using Godot;
using System.Numerics;
using System.Linq;
using TheLiquidFire.Extensions;



public partial class RewardView : Node, IAspect {

    // Called when the node enters the scene tree for the first time.

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


	[Export] PackedScene cardConstruct;
	[Export] Node2D rewardNode2D;
	[Export] Node2D deckNode2D;
	[Export] StatusView statusView;
	[Export] DisplayObjectsView displayObjectsView;
	[Export] int rewardAmount = 3;
	
	[Export] public bool storeSETUP;

	private Player p;
    	public override void _EnterTree()
    {	
		container.Awake ();
		actionSystem = container.GetAspect<ActionSystem> ();
		this.AddObserver (OnValidateReward, Global.ValidateNotification<RewardAction> ());
        }

		public override void _ExitTree()
		{	
		this.RemoveObserver (OnValidateReward, Global.ValidateNotification<RewardAction> ());
		}

		public override void _Process(double delta)
			{
		actionSystem.Update ();
			}

		void OnValidateReward (object sender, object args) {
		var action = sender as RewardAction;
		action.perform.viewer = OnPerformRewardView;
	}

	int elementIndent = 3;
	IEnumerator OnPerformRewardView
	 (IContainer game, GameAction action) {
		

		var children = rewardNode2D.GetChildren();

		
		
		if(children.Count < elementIndent)
			elementIndent = children.Count;

		var overlapY = 200f;
		var overlapX = 250f;
		var width = elementIndent * overlapX;
		var xPos = -(width / 2f);
		var yPos = 0f;

		xPos += overlapX / 2;
		
		//var duration = animated ? 0.25f : 0;
		Tween tween = CreateTween();
		for (int i = 0; i < children.Count; ++i)
		{


		if(i % elementIndent == 0 && i > 0){
		 yPos -= overlapY;
		 xPos = -(width / 2f);
		 xPos += overlapX / 2;
		}
			
			var position = new Godot.Vector2(xPos,0);
			
			tween.TweenProperty(children[i], "position", position, 0.00f).SetEase(Tween.EaseType.Out);
		
			
			xPos += overlapX;


		}

		

	//	topBounds = node.Position.Y - overlapY;
	//	botBounds = yPos - overlapY;
		
		

		while (tween != null && tween.IsRunning())
			yield return null;

		rewardNode2D.Visible = true;
		
		

		for (int i = 0; i < children.Count; ++i)
		{
			
			
			children[i].GetChild<CardView>(0).button.Call("callInstantiate");

		}

		
	}
	public override void _Ready()
	{
	
		SaveFactory.SaveCurrentScene("res://Scenes/RewardScene.tscn");

		initialized = false;
		LoadFileIFEmpty();
		LoadFileTOWeightedList(DataManager.rarityWeightFilePath);
		LoadFileTOWeightedList(DataManager.lootGroupWeightFilePath);
		GenerateCardRewardPack();

		
		SetUpPlayerCards();

		var action = new RewardAction ();
		container.Perform(action);

	}
	public void _on_button_down(){
		if(storeSETUP)
		SceneSwitcher.node.SwitchScene("res://Scenes/MapScene.tscn");
	}
	private void SetUpPlayerCards()
    {
		
	p = new Player(0);

	List<Card> deck = new();

		if(FileFactory.Contains(DataManager.playerdeckPath, "deck")){
			
			deck = DeckFactory.CreateDeck(DataManager.playerdeckPath, p.index);

		}else{

			deck = DeckFactory.CreateDeck (DataManager.placeHolderDeck, p.index);
		}

		p [Zones.Deck].AddRange (deck);
	

		if(storeSETUP)
		Draw(p);
		
    }

	private void Draw(Player player){
		
		for (int i = 0; i < player[Zones.Deck].Count; ++i) {

				var instance = cardConstruct.Instantiate();
				deckNode2D.AddChild(instance);
				instance.GetChild<CardView>(0).card = player[Zones.Deck][i];
				instance.GetChild<CardView>(0).UpdateText();
	

		}
		displayObjectsView.LayoutObjects(deckNode2D, 99).MoveNext();

	}

	
	//File Paths that are edited
	

	void LoadFileIFEmpty(){
		

		if(!FileFactory.Contains(DataManager.rarityWeightFilePath,"list")){

		var editFile =  Godot.FileAccess.Open(DataManager.rarityWeightFilePath,Godot.FileAccess.ModeFlags.Write);

		var file = Godot.FileAccess.Open(DataManager.baseRarityFilePath, Godot.FileAccess.ModeFlags.Read);
		var fileText = file.GetAsText();
		file.Close();

		editFile.StoreString(fileText);

		editFile.Close();

		}

		if(!FileFactory.Contains(DataManager.lootGroupWeightFilePath,"list")){

		var editFile =  Godot.FileAccess.Open(DataManager.lootGroupWeightFilePath,Godot.FileAccess.ModeFlags.Write);


		editFile.StoreString("{");
		editFile.StoreString("\n\"list\": [");
		
		
		foreach	(var dis in SceneSwitcher.node.dispositions){
		DirAccess dir = DirAccess.Open("res://Data/PackCollection/Player/DeckPacks/LootPacks/" + dis.ToString() + "/");
		
		var files = dir.GetFiles();
		
		foreach(var f in files){

			var str = "res://Data/PackCollection/Player/DeckPacks/LootPacks/" + dis.ToString() + "/" + f;

			editFile.StoreString("\n{");
			editFile.StoreString("\n\"name\": ");
			editFile.StoreString("\"" + str + "\"");
			editFile.StoreString("\n\"roll_weight\": ");
			editFile.StoreString("\""+ "100" +"\"");
			editFile.StoreString("\n}");

		}
		
		editFile.StoreString("\n]}");
		editFile.Close();

		}
		}
	}
	


	void GenerateCardRewardPack(){

		
		if(!FileFactory.Contains(DataManager.loadedLootFilePath, "deck")){
		
			var editfile =  Godot.FileAccess.Open(DataManager.loadedLootFilePath,Godot.FileAccess.ModeFlags.Write);
			editfile.StoreString("{ \"deck\": [");
		
		   for(int i = 0; i < rewardAmount; i++){



			
			
				var rarity = WeightRandomSelection.PickAWeightedItem(DataManager.rarityWeightFilePath);
				var lootGroup = WeightRandomSelection.PickAWeightedItem(DataManager.lootGroupWeightFilePath);
				
				
				var id = ChooseACard(rarity,lootGroup);
				
				var card = DeckFactory.CreateCard(id, 0, null);
				
				editfile.StoreString("\"" + id + "\"");
				var instance = cardConstruct.Instantiate();
				
				rewardNode2D.AddChild(instance);

				CardView cardView = instance.GetChild<CardView>(0);
				cardView.card = card;
				
				statusView.GenerateAllStatuses(cardView);
				


				if(storeSETUP)
					GeneratePrice(cardView.card);


				instance.GetChild<CardView>(0).UpdateText();
				
				
				
		   }
		   
			editfile.StoreString("\n ]}");
			editfile.Close();

		}else{
			
			var deck = DeckFactory.CreateDeck(DataManager.loadedLootFilePath, 0);
			foreach(var card in deck){
				var instance = cardConstruct.Instantiate();
				rewardNode2D.AddChild(instance);

				CardView cardView = instance.GetChild<CardView>(0);
				cardView.card = card;

				statusView.GenerateAllStatuses(cardView);


				if(storeSETUP)
					GeneratePrice(cardView.card);

				instance.GetChild<CardView>(0).UpdateText();
			}
		
		}
	}

	public void GeneratePrice(Card card)
    {
		var unitPrice = (Unit)card;
		unitPrice.money += 100;
	}

	public void ReducePrice(CardView select, CardView target){

		if(select == null || target == null)
			return;
			
		if(select.GetParent().GetParent().Name != "DeckNode" || target.GetParent().GetParent().Name != "ObjectNode")
			return;


		var selectUnit = (Unit)select.card;
		var targetUnit = (Unit)target.card;

		var selectM = selectUnit.money;
		var targetM = targetUnit.money;

		if(targetUnit.money <= 0)
			return;

		selectUnit.money = Mathf.Max(0, selectUnit.money - targetUnit.money);
		targetUnit.money = Mathf.Max(0, targetM - selectM);

		select.UpdateText();
		target.UpdateText();

		if(targetUnit.money <= 0){
		SelectCard(targetUnit);
		target.button.Call("callFree");
		}
		
	}


	public string ChooseACard(Item rarity, Item lootGroup)
    {
		
		string group = lootGroup.name;
		
		GD.Print("GROUP " + group);

		var file = Godot.FileAccess.Open(group, Godot.FileAccess.ModeFlags.Read);
		
		var fileText = file.GetAsText();
		var contents = MiniJSON.Json.Deserialize (fileText) as Dictionary<string, object>;
		file.Close();
		

		var array = (List<object>)contents ["deck"];
		var result = new List<string> ();
		foreach (object item in array) {
			var id = (string)item;
			var cardData = DeckFactory.Cards[id];
			var cardRarity = (string)cardData["rarity"];

			if(cardRarity == rarity.name){
				result.Add(id);
			}
			
		}

		if(result.Count == 0){
		
			var rng = RNGFactory.RandiRange(0, array.Count - 1);
			return (string)array[rng];
			
		}

		var cardID = result.Draw();
	
		return cardID;
		
		
	}	

	bool initialized = false;
	private void InitializeNextScene(){	

		if(initialized == true)
			return;

		var file =  Godot.FileAccess.Open(DataManager.mapPath,Godot.FileAccess.ModeFlags.Read);
		var fileText = file.GetAsText();
		var contents = MiniJSON.Json.Deserialize (fileText) as Dictionary<string, object>;
		file.Close();
		var array = (List<object>)contents ["map"];


		var nodeData = (Dictionary<string, object>)array.ElementAt(0);
		var currentMapID = (string)nodeData["currentMapNodeID"];
		
		if(currentMapID == "null")
				return;
	
	    if(currentMapID == "Regular")
			SaveFactory.SaveCurrentScene("res://Scenes/MapScene.tscn");
		else if(currentMapID == "Elite" || currentMapID == "Boss")
			SaveFactory.SaveCurrentScene("res://Scenes/CardAssemblerScene.tscn");

		initialized = true;
	}
	public void SelectCard(Card card)
    {	
		
		FileFactory.ClearFile(DataManager.loadedLootFilePath);
		p.deck.Add(card);
		SaveFactory.SaveDeck(DataManager.playerdeckPath,p);
	//	SaveFactory.SaveCurrentScene("res://Scenes/MapScene.tscn");
		InitializeNextScene();
		List<Item> itemList =  new();
		Rarities rarity = card.rarity;

		if(rarity == Rarities.Common){
			Item item = new("Rare",0.3f);
			itemList.Add(item);
			item = new("Special",0.3f);
			itemList.Add(item);
		}if(rarity == Rarities.Rare){
			Item item = new("Common",0.3f);
			itemList.Add(item);
			item = new("Special",0.3f);
			itemList.Add(item);
		}if(rarity ==  Rarities.Special){
			Item item = new("Common",0.3f);
			itemList.Add(item);
			item = new("Rare",0.3f);
			itemList.Add(item);
		}

		
		WeightRandomSelection.SaveToWeightedCollection(DataManager.rarityWeightFilePath,itemList);
		LoadWeightedListTOFile(DataManager.rarityWeightFilePath);

		
		itemList.Clear();

		var data  = DeckFactory.Cards[card.id];
	

	//	var array = (List<object>)data ["group"];
		var groupList = (string)data["group"];
		var array = groupList.Split("|");

		var disposition = (string)data ["disposition"];
		
		foreach (string str in array) {
			string group = "res://Data/PackCollection/Player/DeckPacks/LootPacks/" + disposition + "/" + str + ".txt";
			Item item = new(group,25);
			itemList.Add (item);
		}


	
		WeightRandomSelection.SaveToWeightedCollection(DataManager.lootGroupWeightFilePath,itemList);
		LoadWeightedListTOFile(DataManager.lootGroupWeightFilePath);


	}

	public void DeleteOptions(CardView activeCardView)
    {
		displayObjectsView.DeleteAllOtherObjects(rewardNode2D,activeCardView.GetParent());
	}
	void LoadWeightedListTOFile(string path){
			
			


            var itemList = WeightRandomSelection.GetItemList(path);
		
			if(itemList != null){
				var file = Godot.FileAccess.Open(path, Godot.FileAccess.ModeFlags.Write);
				
				
				file.StoreString("{");
				file.StoreString("\n\"list\": [");

		foreach (Item item in itemList) {
				file.StoreString("\n{");
				file.StoreString("\n\"name\": ");
				file.StoreString("\"" +  item.name  + "\"");
				file.StoreString("\n\"roll_weight\": ");
				file.StoreString("\""+ item.roll_weight +"\"");
				file.StoreString("\n}");

		}
			file.StoreString("\n]}");
			file.Close();




				file.Close();
			}


	}

	void LoadFileTOWeightedList(string path){
	
			var file = Godot.FileAccess.Open(path, Godot.FileAccess.ModeFlags.Read);
            var fileText = file.GetAsText();
            var dict = MiniJSON.Json.Deserialize (fileText) as Dictionary<string, object>;
            file.Close();

			
            var array = (List<object>)dict ["list"];
            
			List<Item> itemList = new();
            foreach (object entry in array) {
               
                var data = (Dictionary<string, object>)entry;
				
				string name = (string)data["name"];
				string roll_string = (string)data["roll_weight"];
				float roll_weight = float.Parse(roll_string);
                 Item item = new(name, roll_weight);


		
                itemList.Add(item);
            }


            WeightRandomSelection.SaveToWeightedCollection(path, itemList);
			WeightRandomSelection.CalculateWeightedList(path);
	}

	
	private void ResetWeightedSystem(){

		var editFile =  Godot.FileAccess.Open(DataManager.rarityWeightFilePath,Godot.FileAccess.ModeFlags.Write);

		editFile.StoreString("");

		editFile.Close();


		editFile =  Godot.FileAccess.Open(DataManager.lootGroupWeightFilePath,Godot.FileAccess.ModeFlags.Write);

		
		editFile.StoreString("");

		editFile.Close();

		WeightRandomSelection.RemoveFromWeightCollection(DataManager.rarityWeightFilePath);
		WeightRandomSelection.RemoveFromWeightCollection(DataManager.lootGroupWeightFilePath);

	}	
	
}
