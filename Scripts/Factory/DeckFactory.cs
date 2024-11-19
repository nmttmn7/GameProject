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

	public static Dictionary<string, Dictionary<string, object>> Statuses {
		get { 
			if (_statuses == null) {
				_statuses = LoadSTATUSCollection ();
			}
			return _statuses;
		}
	}
	

	private static Dictionary<string, Dictionary<string, object>> _statuses = null;

	private static Dictionary<string, Dictionary<string, object>> _cards = null;
	private static Dictionary<string, object> cardData;

	public static string cardCompendiumPath = "res://UserData/Cards/CardCollection/CardCompendium.txt";


	private static void CreateCollection(){
	

	var file =  Godot.FileAccess.Open(cardCompendiumPath,Godot.FileAccess.ModeFlags.Write);
	
	file.StoreString("{ \"cards\": [");

	var folder = "res://UserData/Cards/CardCollection/";
	
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
			LoadGroup(cardData);
			result.Add (id, cardData);
		}
		return result;
	}

	private static void DeleteGroupFiles(){
		var dir = DirAccess.Open("res://UserData/Cards/Player/DeckPacks/LootPacks/");
		var folders = dir.GetDirectories();

		foreach(var disposition in folders){
			SectionedFiles("res://UserData/Cards/Player/DeckPacks/LootPacks/" + disposition);
		}
		
		
	}

	private static void SectionedFiles(string str)
    {
		DirAccess dir = DirAccess.Open(str);
		var files = dir.GetFiles();
		foreach(var f in files){
		
		FileFactory.RemoveFile(dir, str + "/" + f);

		}
	}
    private static void LoadGroup(Dictionary<string, object> cardData)
    {	
		var id = (string)cardData["id"];	
        var disposition = (string)cardData["disposition"];
		var group = (List<object>)cardData["group"];

		foreach(var section in group){
		
		var text = Godot.FileAccess.Open("res://UserData/Cards/Player/DeckPacks/LootPacks/" + disposition + "/" + section.ToString() + ".txt", Godot.FileAccess.ModeFlags.Read);

		var fileText = "";
		
		if(text != null){
		fileText = text.GetAsText();
		text.Close();
		
		fileText = fileText.Remove(fileText.Length - 3);
		}
		
		var file = Godot.FileAccess.Open("res://UserData/Cards/Player/DeckPacks/LootPacks/" + disposition + "/" + section.ToString() + ".txt", Godot.FileAccess.ModeFlags.Write);

		if(text == null)
			file.StoreString("{ \"deck\": [");

		file.StoreString(fileText);
		
		file.StoreString("\"" + id + "\""+ ",");
		file.StoreString("\n]}");
		file.Close();
	

		}
		//"res://UserData/Cards/Player/DeckPacks/LootPacks/" + disposition + "/";
    }

	private static Dictionary<string, Dictionary<string, object>> LoadSTATUSCollection () {
		
		var file = Godot.FileAccess.Open("res://UserData/StatusCompendium.txt", Godot.FileAccess.ModeFlags.Read);
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
		var fileText = file.GetAsText();
		var contents = MiniJSON.Json.Deserialize (fileText) as Dictionary<string, object>;
		file.Close();

		var array = (List<object>)contents ["deck"];
		var result = new List<Card> ();
		foreach (object item in array) {

			Card card;
			GD.Print("ITEM  = " + item);
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
		AddAfflictions(card, cardData);
		return card;
	}
	
	private static void AddAfflictions(Card card, Dictionary<string, object> data)
    {
		
		if (data.ContainsKey("afflictions") == false)
			return;

	
		
		var afflictions = (List<object>)data ["afflictions"];

		
		foreach (object entry in afflictions) {
			var statusData = (Dictionary<string, object>)entry;
			Status status = new();
			status.Load(statusData);
			
			AddAugment(card, status, Statuses[status.id]);
		}
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
		foreach (object entry in abilities) {
			var abilityData = (Dictionary<string, object>)entry;
			Ability ability = AddAbility (abilityData);
			AddSelector (ability, abilityData);
			AddStatus(ability,abilityData);
			AddCondition(ability,abilityData);

			ability.container = card;
			ability.card = card;
			ability.abilityRoot = abilityRoot;
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
		card.description.Add(str);
		
	
		


		
	}
	private static Ability AddAbility (Dictionary<string, object> data) {
		var ability = new Ability();
		ability.actionName = (string)data["action"];
		ability.userInfo = data["info"];
		ability.abilityCount = Convert.ToInt32 (data["count"]);
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


	private static void AddStatus(Ability ability, Dictionary<string, object> data){
		if (data.ContainsKey ("status") == false)
			return;
		
		var statusData = (Dictionary<string, object>)data["status"];
		var status = ability.AddAspect<Status>();
		status.container = ability;
		status.ability = ability;
		status.Load(statusData);

	}
	
	public static void AddAugment(Card card, Status condition, Dictionary<string, object> data){
		
		var aug = card.GetAspect<Augment>();

		if(aug == null)
			aug = card.AddAspect<Augment>();
	
	string id = (string)data["id"];
	string evokeType = (string)data["evokeType"];
	string spriteString = (string)data["sprite"];
	string decr = (string)data["decr"];
		

		var abilities = (List<object>)data["abilities"];

		AbilityRoot root = new();
		root.container = card;

		if(condition != null){
				

				var newCondition = root.AddAspect<Status>();
				//newCondition.value = condition.increase;
				newCondition.name = condition.name;
				newCondition.sprite = spriteString;
				newCondition.description = condition.description;
				newCondition.id = condition.id;
				newCondition.evokeType = evokeType;
				

				if(condition.decrease == null)
				newCondition.decrease = decr;
				
				else{
				newCondition.decrease = condition.decrease;
				
				}
				if(condition.value != 0)
				newCondition.value = condition.value;

				newCondition.modifier = condition.modifier;
				newCondition.statusType = condition.statusType;
				newCondition.valueTOability = condition.valueTOability;
				newCondition.permanent = condition.permanent;
				newCondition.flip = condition.flip;

				newCondition.cardID = condition.cardID;
			}

		foreach (object abilityEntry in abilities) {

			var abilityData = (Dictionary<string, object>)abilityEntry;
			Ability ability = AddAbility (abilityData);

			AddStatus(ability,abilityData);
			AddSelector (ability, abilityData,false);
			AddCondition(ability,abilityData);



			ability.container = card;
			ability.card = card;
			ability.abilityRoot = root;
			root.abilityChain.Add(ability);

			

			}

			aug.statusPairs.Add(id,root);

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



}



