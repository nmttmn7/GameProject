using Godot;
using System;
using System.Collections.Generic;
using System.IO;

public static class SaveFactory {

	//public static string userName = "Wolf";

	public static void CreateDirectory(string path, string newPath){
		var dir = Godot.DirAccess.Open(path);
			if(!dir.DirExists(newPath)){
				var newDir = dir.MakeDir(newPath);
			}
			
	}

	public static void SaveDisposition(){		
		var file = Godot.FileAccess.Open("res://UserData/Cards/Player/Disposition.txt", Godot.FileAccess.ModeFlags.Write);
		file.StoreString("[");
		foreach(var d in SceneSwitcher.node.dispositions)
			file.StoreString("\""+ d.ToString() + "\"" + ",");
		file.StoreString("]");
		file.Close();
	}

	public static void LoadDisposition(){		
		var file = Godot.FileAccess.Open("res://UserData/Cards/Player/Disposition.txt", Godot.FileAccess.ModeFlags.Read);
		var fileText = file.GetAsText();
		var list = MiniJSON.Json.Deserialize (fileText) as List<object>;
		file.Close();

		foreach(var d in list){
			SceneSwitcher.node.dispositions.Add(d.ToString());
		}
		
		
		
	}
	public static void SaveDeck(string path, Player player){

		var file = Godot.FileAccess.Open(path, Godot.FileAccess.ModeFlags.Write);
		//var fileText = file.GetAsText();

		file.StoreString("{ \"deck\": [");

		var zones = new Zones[] { 
			 
			Zones.Deck, 
			Zones.Hand,
			Zones.Discard,
			Zones.Graveyard,  

		};
		foreach (Zones zone in zones) {

				for(int i = 0; i < player[zone].Count; i++){
						SaveCard(player[zone][i],file);
				}
		}


		file.StoreString("\n]}");
		file.Close();
	}
	public static void SaveReward(Card card)
    {

		var text = Godot.FileAccess.Open("res://UserData/Cards/Player/Deck.txt", Godot.FileAccess.ModeFlags.Read);
		var fileText = text.GetAsText();
		text.Close();
		
		fileText = fileText.Remove(fileText.Length - 3);
		
		
		var file = Godot.FileAccess.Open("res://UserData/Cards/Player/Deck.txt", Godot.FileAccess.ModeFlags.Write);
		file.StoreString(fileText);
		SaveCard(card, file);
		file.StoreString("\n]}");
		file.Close();
	}
	private static void SaveCard(Card card, Godot.FileAccess file)
    {

		var temp = card.GetAspect<Temp>();
		
		if(temp != null)
			return;

		var aug = card.GetAspect<Augment>();
		var unit = (Unit)card;
		
		

		
		//file.StoreString("\"" + unit.id + "\"" + ",");
		

	

		file.StoreString("{");
		file.StoreString(unit.Save());

		var target = card.GetAspect<Target>();
		if(target != null)
		file.StoreString(target.Save());

		SaveAbilities(card, file);
		SaveAfflictions(card,file);
		file.StoreString("}");

		
	}

    private static void SaveAfflictions(Card card, Godot.FileAccess file)
    {
       var augment = card.GetAspect<Augment>();
		if(augment != null)
		file.StoreString(augment.Save());
    }

    private static void SaveAbilities(Card card, Godot.FileAccess file){

			var abilityRoot = card.GetAspect<AbilityRoot>();
			if(abilityRoot != null){
			
			file.StoreString("\n\"abilities\": [");
			foreach(var ability in abilityRoot.abilityChain){
				file.StoreString(ability.Save());
			}

			file.StoreString("\n]");

			}	
	}

	public static void SaveCurrentScene(string sceneConstruct){
		RNGFactory.SaveSeed();
		SaveFactory.SaveTOFile(MainMenu.saveScene, "{" + "\"" + "scene"+ "\"" + ": " + "\"" + sceneConstruct + "\"" + "}");
	}
/*
	public static void AddCardToSaveFile(Card card){
		var file =  Godot.FileAccess.Open(PathFactory.playerDeckPath,Godot.FileAccess.ModeFlags.Read);
		var fileText = file.GetAsText();
		file.Close();

		var Length = fileText.Length;
		if(Length <= 0 || !fileText.Contains("cards")){
			fileText = "{ \"cards\": [";
			fileText += "\n ]}";
		}
		var end = fileText.Find("]}");
	//	GD.Print("END = "+ fileText);
		fileText = fileText.Remove(end);
	
	

		var editfile =  Godot.FileAccess.Open(PathFactory.playerDeckPath,Godot.FileAccess.ModeFlags.Write);
		editfile.StoreString(fileText);
		AddCardToFile(editfile, card);
		editfile.StoreString("\n ]}");
		editfile.Close();

 
	}

	#region SaveCards
	public static void SavePlayerCards(Player player)
    {
	//	var player = match.players[0];
		
		//CreateUserDirectory();
	//	CreateDirectory("res://UserData", userName);
		//CreateCardDirectory();
	//	CreateDirectory("res://UserData/" + userName, "Cards");

		
		var file =  Godot.FileAccess.Open(PathFactory.playerDeckPath,Godot.FileAccess.ModeFlags.Write);
		FileFactory.ClearFile(PathFactory.playerDeckPath);
		file.StoreString("{ \"cards\": [");
		
		foreach(var card in player.deck)
		AddCardToFile(file, card);

		foreach(var card in player.hand)
		AddCardToFile(file, card);

		foreach(var card in player.discard)
		AddCardToFile(file, card);


		file.StoreString("\n ]}");
		file.Close();

		//AddCardToGraveFile(match);
	
	}

	public static void AddCardToFile(Godot.FileAccess file, Card card)
    {
		file.StoreString("\n {");
		
		file.StoreString(card.Save());
		AddTargetToFile(file, card);
		AddAbilityToFile(file, card);
		
		AddAugmentToFile(file,card);
		file.StoreString("\n }");
	}
	private static void AddAugmentToFile(Godot.FileAccess file, Card card){

		var augment = card.GetAspect<Augment>();
		
		if(augment == null)
			return;


		file.StoreString("\n    \"augments\": [");

		
		foreach(var pair in augment.pairs){
			file.StoreString("\n {");
		file.StoreString("\n\"evokeName\": " + "\"" + pair.Key + "\"");	
		AddAbilitiesToFile(file,pair.Value);
			file.StoreString("\n }");
		}

		file.StoreString("\n],");
	}

	private static void AddAbilitiesToFile(Godot.FileAccess file, AbilityRoot abilityRoot){

		if(abilityRoot == null)
			return;

		file.StoreString("\n    \"abilities\": [");
		foreach (Ability entry in abilityRoot.abilityChain) {
			file.StoreString("\n {");
			file.StoreString(entry.Save());
			AddTargetSelectorToFile(file,entry);
			AddConditionToFile(file,entry);
			file.StoreString("\n }");
			}
		file.StoreString("\n ]");
	}	

	
	private static void AddTargetToFile(Godot.FileAccess file, Card card)
    {
		var target = card.GetAspect<Target>();
		
		if(target == null)
			return;

		file.StoreString("\n    \"target\": {");
		file.StoreString(target.Save());
		file.StoreString("\n},");
	}
	private static void AddAbilityToFile(Godot.FileAccess file, Card card){

		var abilityRoot = card.GetAspect<AbilityRoot>();
		
		if(abilityRoot == null)
			return;

		file.StoreString("\n    \"abilities\": [");

		
		foreach (Ability entry in abilityRoot.abilityChain) {
		file.StoreString("\n {");
		file.StoreString(entry.Save());
		AddTargetSelectorToFile(file,entry);
	//	AddStatusToFile(file,entry);
		AddConditionToFile(file,entry);
		file.StoreString("\n }");
		}
		file.StoreString("\n ]");
	}	

	private static void AddConditionToFile(Godot.FileAccess file, Ability entry)
    {
		var condition = entry.GetAspect<Condition>();

		if(condition == null)
			return;

		file.StoreString("\n    \"condition\": {");
		file.StoreString("\n" + "\"type\": " + "\"" + condition.type + "\"");
		file.StoreString("\n" + "\"incr\": " + "\"" + condition.increase + "\"");
		file.StoreString("\n }");
	}
	public static void AddTargetSelectorToFile(Godot.FileAccess file, Ability ability){

		var targetSelector = ability.GetAspect<ITargetSelector>();
		
		if(targetSelector == null)
			return;

		file.StoreString("\n    \"targetSelector\": {");

		file.StoreString(targetSelector.Save());
		
		file.StoreString("\n }");
	}	


	public static void AddCardToGraveFile(Match match){

		var player = match.players[0];
		var file =  Godot.FileAccess.Open("res://UserData/Cards/Graveyard.txt",Godot.FileAccess.ModeFlags.Write);

		file.StoreString("{ \"cards\": [");
	
		foreach(var card in player.graveyard)
		AddCardToFile(file, card);

		file.StoreString("\n ]}");
		file.Close();
	}
	#endregion
	*/

	public static void SaveTOFile(string path,string data){

		var file =  Godot.FileAccess.Open(path,Godot.FileAccess.ModeFlags.Write);
		file.StoreString(data);
		file.Close();

	}
	#region MapSave
	public static void SavePlayerMap(MapView mapConstruct)
    {


		
		var file =  Godot.FileAccess.Open(MapView.mapPath,Godot.FileAccess.ModeFlags.Write);
		file.StoreString("{ \"map\": [");
		file.StoreString("\n{");
	
		if(mapConstruct.currentNode != null){
		file.StoreString("\n" + "\"currentMapNodeX\": " + "\"" + mapConstruct.currentNode.jaggedX + "\"");
		file.StoreString("\n" + "\"currentMapNodeDepth\": " + "\"" + mapConstruct.currentNode.depth + "\"");
		file.StoreString("\n" + "\"currentMapNodeID\": " + "\"" + mapConstruct.currentNode.mapID + "\"");
		}else{
			file.StoreString("\n" + "\"currentMapNodeX\": " + "\"" + "null" + "\"");
		file.StoreString("\n" + "\"currentMapNodeDepth\": " + "\"" + "null" + "\"");
		}
		file.StoreString("\n" + "\"mapWorld\": " + "\"" + mapConstruct.mapWorld + "\"");
		file.StoreString("\n" + "\"playerDepth\": " + "\"" + mapConstruct.playerDepth + "\"");
		file.StoreString("\n}");
		for(int i = 0; i < mapConstruct.jaggedMap.Length; i++){
				var bottom = mapConstruct.jaggedMap[i];
			for(int b = 0; b < bottom.Length; b++){
			
			AddMapNodeToFile(file,mapConstruct.jaggedMap[i][b].GetNode<MapNode>("MapNode"));

			}
		}

		file.StoreString("\n ]}");
		file.Close();
	}

	public static void AddMapNodeToFile(Godot.FileAccess file, MapNode mapNode)
    {
		file.StoreString("\n {");
		
		file.StoreString(mapNode.Save());
		
		file.StoreString("\n }");
	}

   

    #endregion



}
