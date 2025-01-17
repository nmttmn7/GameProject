using System;
using System.Collections;
using System.Collections.Generic;
using Godot;

using TheLiquidFire.AspectContainer;

public class Card : TheLiquidFire.AspectContainer.Container{
	public string id;
	public string name;
	public Rarities rarity;
	public string spritePath;
	public int cost;
	public string c;
	public int orderOfPlay = int.MaxValue;
	public int ownerIndex;
	public Zones zone = Zones.Deck;

	public virtual void Load (Dictionary<string, object> data) {
		id = (string)data ["id"];
		name = (string)data ["name"];


		var rarityData = (string)data ["rarity"];
		bool isValid = Enum.TryParse(rarityData, out Rarities rarityColl);
		if(isValid) {
			rarity = rarityColl;
		}else{
			throw new NotImplementedException();
		}
		

		
		spritePath = (string)data ["sprite"];

		cost = System.Convert.ToInt32(data["cost"]);

		if(data.ContainsKey("C"))
		c = (string)data ["C"];;
	}

	public virtual string Save(){

		string text = "";
		text += "\n\"id\": " + "\"" + id + "\","; 
		text += "\n\"type\": " + "\"" + GetType() + "\","; 
		text += "\n\"rarity\": " + "\"" + rarity + "\",";
		text += "\n\"name\": " + "\"" + name + "\","; 
		text += "\n\"sprite\": " + "\"" + spritePath + "\","; 		
		text += "\n\"cost\": " + "\"" +  cost + "\","; 
		return text;
	}
}