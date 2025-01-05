using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using TheLiquidFire.AspectContainer;
using TheLiquidFire.Notifications;

public class Status : TheLiquidFire.AspectContainer.Container, IAspect {

#region BasicVariables
#endregion
	public IContainer container { get; set; }
	public string id {get; set;}
	public string name {get; set;}
	public string description {get; set;}
    public string evokeType {get; set;}
	public string sprite {get; set;}
    public int value{get; set;}
    public int decrease {get; set;}
	public StatusTypes statusType {get; set;}
	
	public bool flip {get; set;}

	public string color {get;set;}
	
	public AbilityRoot abilityRoot = new();
	
    public void Load(Dictionary<string, object> data, Card target, Ability castedAbility = null, StatusSystem statusSystem = null) {
		
		container = target;
		abilityRoot.container = this;

		if(data.ContainsKey("id")){
		id = (string)data["id"];
		id = id.ToLower();
		}

		if(data.ContainsKey("name"))
			name = (string)data["name"];

		if(data.ContainsKey("sprite"))
			sprite = (string)data["sprite"];

		if(data.ContainsKey("evokeType")){
			string temp = (string)data["evokeType"];

			if(evokeType == null)
				evokeType += ApplyColorFilter(temp);
			else if(!temp.Contains(evokeType)){
				if(temp.Contains("Set")){
				temp = temp.Replace("Set","");
				evokeType = ApplyColorFilter(temp);
				}else
				evokeType += ApplyColorFilter(temp);

			}
		}

		



		if(data.ContainsKey("description"))
			description +=  ApplyColorFilter((string)data["description"]);

			

		

		if(data.ContainsKey("incr")){

			int statusINCREMENT = statusSystem.ParseAbilityInfo((string)data ["incr"], target, castedAbility);


			if(data.ContainsKey("modifier"))
				    value *= statusINCREMENT;
				else
				    value += statusINCREMENT;
		}
	
	
		if(data.ContainsKey("decr"))
			decrease = Int32.Parse((string)data ["decr"]);
		
		
		if(data.ContainsKey("value"))
		    value = Int32.Parse((string)data ["value"]);
		

		


		

		if(data.ContainsKey("flip"))
			flip = (bool)data ["flip"];
		
		if(data.ContainsKey("color"))
			color = (string)data["color"];

		string typeData = ""; 

		if(data.ContainsKey("type"))
			typeData = (string)data ["type"];
		
		

		
	   	if(typeData.Length > 0){
		bool isValid = Enum.TryParse(typeData, out StatusTypes typeStat);
		if(isValid) {
			statusType = typeStat;
		}else{
			throw new NotImplementedException();
		}

		}
		
		if(data.ContainsKey("abilities"))
			DeckFactory.AddAbilities(abilityRoot,target,data);
	}
	private string ApplyColorFilter(string temp){
		

			if(temp.Contains("color"))
				return temp;
			else
				return "[color=" +  color  +  "]" + temp + "[/color]";

				
	}
	
	public string Save(){
		string text = "";
		
		text += "{";
		text += "\n\"id\": " + "\"" + id + "\","; 
		text += "\n\"name\": " + "\"" + name + "\",";
		text += "\n\"type\": " + "\"" + statusType + "\","; 
		text += "\n\"sprite\": " + "\"" + sprite + "\","; 
		text += "\n\"evokeType\": " + "\"" + evokeType + "\",";
		text += "\n\"description\": " + "\"" + description + "\","; 
		text += "\n\"decr\": " + "\"" + decrease + "\","; 
		text += "\n\"value\": " + "\"" + value + "\","; 
		text += "\n\"flip\": " + "" + flip.ToString().ToLower() + ",";
		text += "\n\"color\": " + "" + color + ",";
		if(abilityRoot.abilityChain.Count > 0){
			text += "\n\"abilities\": [";
		foreach(var ability in abilityRoot.abilityChain)
			ability.Save();
		text += "]";
		}

		text += "}";
	
		return text;
	}


}
