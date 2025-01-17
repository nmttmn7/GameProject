using System.Collections;
using System.Collections.Generic;
using Godot;
using System;
using System.Linq;

public static class DeckFactory {
	// Maps from a Card ID, to the Card's Data
	public static Dictionary<string, Dictionary<string, object>> Cards {
		get { 
			if (_cards == null) {
				_cards = LoadCARDCollection (cardCompendiumPath);
			}
			return _cards;
		}
	}

	public static Dictionary<string, Dictionary<string, object>> Afflictions {
		get { 
			if (_afflictions == null) {
				_afflictions = LoadAfflictionsCollection ();
			}
			return _afflictions;
		}
	}
	

	private static Dictionary<string, Dictionary<string, object>> _afflictions = null;

	private static Dictionary<string, Dictionary<string, object>> _cards = null;
	private static Dictionary<string, object> cardData;

	public static string cardCompendiumPath = "res://Data/CardCollection/CardCompendium.txt";


	private static void CreateCollection(){
	

	var file =  Godot.FileAccess.Open(cardCompendiumPath,Godot.FileAccess.ModeFlags.Write);
	
	file.StoreString("{ \"cards\": [");

	var folder = "res://Data/CardCollection/Units/";
	
	var dir = DirAccess.Open(folder);
	var allDir = dir.GetDirectories();
	foreach(var d in allDir){
	var newDir = DirAccess.Open(folder + d);
	var allFiles = newDir.GetFiles();

	foreach(var loadFile in allFiles){
	file.StoreString(Godot.FileAccess.Open(folder + d + "/" + loadFile,Godot.FileAccess.ModeFlags.Read).GetAsText());
	}

	}

	
	
	

	file.StoreString("\n ]}");
	file.Close();

	}
	private static Dictionary<string, Dictionary<string, object>> LoadCARDCollection (string path) {
		
		CreateCollection();

		DeleteGroupFiles();

		var file = Godot.FileAccess.Open(path, Godot.FileAccess.ModeFlags.Read);
		var fileText = file.GetAsText();
		var dict = MiniJSON.Json.Deserialize (fileText) as Dictionary<string, object>;
		file.Close();
		var array = (List<object>)dict ["cards"];
		var result = new Dictionary<string, Dictionary<string, object>> ();
		foreach (object entry in array) {
			var cardData = (Dictionary<string, object>)entry;
			var id = (string)cardData["id"];
			if(!id.Contains("CREATEDCARD"))
				LoadGroup(cardData);

			result.Add (id, cardData);
		}
		return result;
	}

	private static void DeleteGroupFiles(){
		var dir = DirAccess.Open("res://Data/PackCollection/Player/DeckPacks/LootPacks/");
		var folders = dir.GetDirectories();

		foreach(var disposition in folders){
			SectionedFiles("res://Data/PackCollection/Player/DeckPacks/LootPacks/" + disposition);
		}
		
		
	}

	private static void SectionedFiles(string str)
    {
		DirAccess dir = DirAccess.Open(str);
		var files = dir.GetFiles();
		
		foreach(var f in files){
		
		dir.Remove(str + "/" + f);
		
		}
		

		
	}

    private static void LoadGroup(Dictionary<string, object> cardData)
    {	
		var id = (string)cardData["id"];	
        var disposition = (string)cardData["disposition"];
	//	var group = (List<object>)cardData["group"];
		var groupList = (string)cardData["group"];
		var split = groupList.Split("|");
		foreach(var section in split){
		
		var text = Godot.FileAccess.Open("res://Data/PackCollection/Player/DeckPacks/LootPacks/" + disposition + "/" + section.ToString() + ".txt", Godot.FileAccess.ModeFlags.Read);

		var fileText = "";
		
		if(text != null){
		fileText = text.GetAsText();
		text.Close();
		
		fileText = fileText.Remove(fileText.Length - 3);
		}
		
		var file = Godot.FileAccess.Open("res://Data/PackCollection/Player/DeckPacks/LootPacks/" + disposition + "/" + section.ToString() + ".txt", Godot.FileAccess.ModeFlags.Write);

		if(text == null)
			file.StoreString("{ \"deck\": [");

		file.StoreString(fileText);
		
		file.StoreString("\"" + id + "\""+ ",");
		file.StoreString("\n]}");
		file.Close();
	

		}
		//"res://UserData/Cards/Player/DeckPacks/LootPacks/" + disposition + "/";
    }

	private static Dictionary<string, Dictionary<string, object>> LoadAfflictionsCollection () {
		
		var file = Godot.FileAccess.Open("res://Data/AfflictionCollection/AfflictionCompendium.txt", Godot.FileAccess.ModeFlags.Read);
		var fileText = file.GetAsText();
		var dict = MiniJSON.Json.Deserialize (fileText) as Dictionary<string, object>;
		file.Close();
		

		var array = (List<object>)dict ["statuses"];
		var result = new Dictionary<string, Dictionary<string, object>> ();
		foreach (object entry in array) {
			var statusData = (Dictionary<string, object>)entry;
			var id = (string)statusData["id"];
			result.Add (id, statusData);
		}
		return result;
	}


	public static List<Card> LoadDeck(string path, int ownerIndex){

		var file = Godot.FileAccess.Open(path, Godot.FileAccess.ModeFlags.Read);
		var fileText = file.GetAsText();
		var contents = MiniJSON.Json.Deserialize (fileText) as Dictionary<string, object>;
		file.Close();

		var array = (List<object>)contents ["deck"];
		var result = new List<Card> ();
		foreach (object item in array) {
			var cardData = (Dictionary<string, object>)item;
			var card = CreateCard (null, ownerIndex, cardData);
			result.Add (card);
		}
		return result;


	}
	public static List<Card> CreateDeck(string path, int ownerIndex) {
		var file = Godot.FileAccess.Open(path, Godot.FileAccess.ModeFlags.Read);
		GD.Print(path);
		var fileText = file.GetAsText();
		var contents = MiniJSON.Json.Deserialize (fileText) as Dictionary<string, object>;
		file.Close();

		var array = (List<object>)contents ["deck"];
		var result = new List<Card> ();
		foreach (object item in array) {

			Card card;
			if(!item.ToString().Contains("Collections")){
			var id = (string)item;
			card = CreateCard (id, ownerIndex, null);

			}else{
			var cardData = (Dictionary<string, object>)item;
			card = CreateCard (null, ownerIndex, cardData);
			}

			result.Add (card);
		}
		return result;
	}

	public static Card CreateCard(string id, int ownerIndex, Dictionary<string, object> data) {
		if(data == null){
		 	cardData = Cards [id];
		}else{
	 		cardData = data;
		}
		Card card = CreateCard (cardData, ownerIndex);
		AddTarget (card, cardData);
		AddAbilities (card, cardData);
		AddMechanics (card, cardData);
		
		return card;
	}
	
	



	private static Card CreateCard (Dictionary<string, object> data, int ownerIndex) {
		var cardType = (string)data["type"];
		var type = Type.GetType (cardType);
		var instance = Activator.CreateInstance (type) as Card;
		instance.Load (data);
		instance.ownerIndex = ownerIndex;
		return instance;
	}

	private static void AddTarget (Card card, Dictionary<string, object> data) {
		if (data.ContainsKey ("target") == false)
			return;
		var targetData = (Dictionary<string, object>)data ["target"];
		var target = card.AddAspect<Target> ();
		var allowedData = (Dictionary<string, object>)targetData["allowed"];
		target.allowed = new Mark (allowedData);
		var preferredData = (Dictionary<string, object>)targetData["preferred"];
		target.preferred = new Mark (preferredData);
	}

	private static void AddAbilities (Card card, Dictionary<string, object> data) {
		if (data.ContainsKey ("abilities") == false)
			return;


		var abilityRoot = card.AddAspect<AbilityRoot>();
		var abilities = (List<object>)data ["abilities"];
		var i = 1;
		foreach (object entry in abilities) {
			var abilityData = (Dictionary<string, object>)entry;
			Ability ability = AddAbility (abilityData);
			AddSelector (ability, abilityData);
			AddStatusData(ability,abilityData);
			AddCondition(ability,abilityData);


			if(abilityData.ContainsKey("chainMAX"))
				abilityRoot.chainMAX = (int)abilityData["chainMAX"];
			else
				abilityRoot.chainMAX = 99;


			ability.container = abilityRoot;
			ability.card = card;
			abilityRoot.container = card;
			abilityRoot.abilityChain.Add(ability);
			ability.chainPosition = i;

			AddDescription(ability,card);
			i++;
		}
		
	}

	public static void AddAbilities(AbilityRoot abilityRoot, Card card, Dictionary<string, object> data){
		
		
		var abilities = (List<object>)data ["abilities"];

		foreach (object entry in abilities) {
			var abilityData = (Dictionary<string, object>)entry;
			Ability ability = AddAbility (abilityData);
			AddSelector (ability, abilityData);
			AddStatusData(ability,abilityData);
			AddCondition(ability,abilityData);
			ability.container = abilityRoot;
			ability.card = card;
			abilityRoot.abilityChain.Add(ability);
			
		}


	}

	public static void AddStatusAbilities(Card card, AbilityRoot abilityRoot, Dictionary<string, object> data){

		if (data.ContainsKey ("abilities") == false)
			return;


		
		var abilities = (List<object>)data ["abilities"];
		foreach (object entry in abilities) {
			var abilityData = (Dictionary<string, object>)entry;
			Ability ability = AddAbility (abilityData);
			AddSelector (ability, abilityData);
			AddStatusData(ability,abilityData);
			AddCondition(ability,abilityData);

			ability.container = card;
			ability.card = card;
			abilityRoot.container = card;
			abilityRoot.abilityChain.Add(ability);

			AddDescription(ability,card);
		}

	}
	private static void AddCondition(Ability ability, Dictionary<string, object> data){

		if (!data.ContainsKey ("condition"))
			return;

		Condition condition = ability.AddAspect<Condition> ();

		var conditionData = (Dictionary<string, object>)data["condition"];
		condition.conditionInfo = (string)conditionData["info"];
		condition.comparator = (string)conditionData["comparator"];
		condition.value = (string)conditionData["value"];

		condition.container = ability;
		
		

		
	}

	public static Condition CreateCondition(Ability ability, Dictionary<string, object> data){

		if (!data.ContainsKey ("condition"))
			return null;

		Condition condition = new();

		var conditionData = (Dictionary<string, object>)data["condition"];
		condition.conditionInfo = (string)conditionData["info"];
		condition.comparator = (string)conditionData["comparator"];
		condition.value = (string)conditionData["value"];

		condition.container = ability;
		
		return condition;

		
	}
	private static void AddDescription(Ability ability,Card card){
		
		
		var type = Type.GetType (ability.actionName);
		var instance = Activator.CreateInstance (type) as GameAction;
		var loader = instance as IAbilityLoader;
		string str = "";
		if (loader != null)
		str += loader.LoadText (ability);
		str +="\n" + "\n" + "\n" + "\n";
		ability.description = str;
		
	
		


		
	}
	private static Ability AddAbility (Dictionary<string, object> data) {
		var ability = new Ability();
		ability.actionName = (string)data["action"];
		//ability.userInfo = data["info"];

		ability.info = (string)data ["info"];
		ability.abilityCount = data["count"];

		if (data.ContainsKey ("locked") == true)
			ability.locked = (bool)data["locked"];
		else
			ability.locked = false;

		return ability;
	}

	private static void AddSelector (Ability ability, Dictionary<string, object> data) {
		if (data.ContainsKey ("targetSelector") == false)
			return;
		var selectorData = (Dictionary<string, object>)data["targetSelector"];
		var typeName = (string)selectorData["type"];
		var type = Type.GetType (typeName);
		var instance = Activator.CreateInstance (type) as ITargetSelector;
		instance.Load (selectorData);
		ability.AddAspect<ITargetSelector> (instance);
	}

	private static void AddMechanics (Card card, Dictionary<string, object> data) {
		if (data.ContainsKey ("taunt")) {
			card.AddAspect<Taunt> ();
		}
	}




	private static void AddStatusData(Ability ability, Dictionary<string, object> data){
		if (data.ContainsKey ("status") == false)
			return;
		
		var statusData = (Dictionary<string, object>)data["status"];
		var status = ability.AddAspect<StatusData>();
		status.container = ability;
		status.data = statusData;

	}
	



	
	private static void AddSelector (Ability ability, Dictionary<string, object> data, bool multi) {
		if (data.ContainsKey ("targetSelector") == false)
			return;
		var selectorData = (Dictionary<string, object>)data["targetSelector"];
		var typeName = (string)selectorData["type"];
		var type = Type.GetType (typeName);
		var instance = Activator.CreateInstance (type) as ITargetSelector;
		instance.Load (selectorData);
		ability.AddAspect<ITargetSelector> (instance);
	
	}

	public static void TransformCard(Card target, string cardID) {
		
	
		var cardData = Cards[cardID];
		target.DeleteAspect<AbilityRoot>();
		
		AddAbilities(target,cardData);
		target.spritePath =  (string)cardData ["sprite"];
		target.name = (string)cardData ["name"];

	
	}

	public static bool CombineCards(Card baseCard, CardView deleteCard, bool selected){
		
		AbilityRoot baseRoot = baseCard.GetAspect<AbilityRoot>();
		AbilityRoot deleteRoot = deleteCard.card.GetAspect<AbilityRoot>();
		bool success = false;

		if(selected == false){
		foreach (var ability in deleteRoot.abilityChain){
			if(!ability.locked && baseRoot.abilityChain.Count < baseRoot.chainMAX){
				ability.container = baseCard;
				ability.card = baseCard;
				baseRoot.abilityChain.Add(ability);
				success = true;
			}
		}

		}else{
		Ability deleteAbility = deleteRoot.abilityChain[deleteCard.GetDescriptionIndex()];
		if(!deleteAbility.locked && baseRoot.abilityChain.Count < baseRoot.chainMAX){

		deleteAbility.container = baseCard;
		deleteAbility.card = baseCard;
		baseRoot.abilityChain.Add(deleteAbility);
		success = true;

		}

		}

		if(success)
		return true;
		else
		return false;

	}


}



