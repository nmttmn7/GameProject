using System.Collections;
using System.Collections.Generic;

using TheLiquidFire.AspectContainer;

public class StatusData : Aspect {
	
	public string stringData;
	public Dictionary<string, object> data = new();

	public string InterpretStatus(Dictionary<string, object> statusData){

		string description = "";
		string id = "";

		if(data.ContainsKey("id")){
		id = (string)data["id"];
		id = id.ToLower();
		}
		if(data.ContainsKey("modifier"))
			description += (string)data["modifier"];
		if(data.ContainsKey("incr"))
			description += (string)data["incr"];

		var globalData = DeckFactory.Statuses[id];
			string statSprite = (string)globalData["sprite"];
			description += " [img=40]" + statSprite + "[/img] ";

		

		if(data.ContainsKey("evokeType"))
			description += "[" + (string)data["evokeType"] + "]";

		if(data.ContainsKey("decr"))
			description += "[" + (string)data["decr"] + "]";

		if(data.ContainsKey("abilities"))
			description += "STATUSABILITY TEXT NOT IMPLEMENTED";
		
		
		return description;
			

		
	}

	public string Save(){
		string str = ""; 
		foreach (var item in data) {
			str += "\n\"" + item.Key + "\"" + " : " + "\"" + item.Value.ToString() + "\"";
		}
		return str;
	}
}
