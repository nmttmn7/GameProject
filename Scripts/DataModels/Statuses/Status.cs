using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using TheLiquidFire.AspectContainer;
using TheLiquidFire.Notifications;

public class Status : TheLiquidFire.AspectContainer.Container, IAspect {

	public IContainer container { get; set; }
	public Ability ability {get; set;}

	public string id {get; set;}
	public string name {get; set;}
	public string description {get; set;}
    public string evokeType {get; set;}
	public bool valueTOability {get; set;}
	public string sprite {get; set;}
    public int value{get; set;}
    public object increase {get; set;}
	public string modifier {get; set;}
    public object decrease {get; set;}
	public StatusTypes statusType {get; set;}
	public bool permanent {get; set;}
	public bool flip {get; set;}
	
	
	public string cardID {get; set;}
	public Card polyCard {get; set;}

	public int glitchValue {get; set;}
    public void Load(Dictionary<string, object> data) {
		
		id = (string)data["id"];
		id = id.ToLower();
	
		var statusData = DeckFactory.Statuses[id];
		
		name = (string)statusData["name"];
		sprite = (string)statusData["sprite"];
		evokeType = (string)statusData["evokeType"];
		description = (string)statusData["description"];
	
		increase = (string)data ["incr"];
		
		if(data.ContainsKey("decr"))
			decrease = (string)data["decr"];
		else if(statusData.ContainsKey("decr")){
			decrease = (string)statusData["decr"];
		}else
			decrease = "1";

		if(data.ContainsKey("modifier"))
		modifier = (string)data ["modifier"];
		else
		modifier = "+";
	
		if(statusData.ContainsKey("valueTOability"))
		valueTOability = (bool)statusData ["valueTOability"];
		else
		valueTOability = true;

		if(data.ContainsKey("permanent"))
		permanent = (bool)data ["permanent"];
		else
		permanent = false;

		if(statusData.ContainsKey("flip"))
		flip = (bool)statusData ["flip"];
		else
		flip = false;


		string typeData; 

		if(data.ContainsKey("type"))
			typeData = (string)data ["type"];
		else
			typeData = (string)statusData ["type"];
		

		
	   	
		bool isValid = Enum.TryParse(typeData, out StatusTypes typeStat);
		if(isValid) {
			statusType = typeStat;
		}else{
			throw new NotImplementedException();
		}

		if(data.ContainsKey("cardID"))
		cardID = (string)data["cardID"];
		
		if(data.ContainsKey("value"))
		value = Int32.Parse((string)data ["value"]);
		else
		value = 0;

	}

	public string Save(){
		string text = "";
		

		text += "\n\"id\": " + "\"" + id + "\","; 
		text += "\n\"evokeType\": " + "\"" + evokeType + "\","; 
		text += "\n\"value\": " + "\"" + value + "\","; 
		text += "\n\"incr\": " + "\"" + increase + "\","; 
		text += "\n\"modifier\": " + "\"" + modifier + "\","; 
		text += "\n\"decr\": " + "\"" + decrease + "\","; 
		text += "\n\"statusType\": " + "\"" + statusType + "\",";
		text += "\n\"valueTOability\": " + "" + valueTOability.ToString().ToLower() + ","; 
		text += "\n\"flip\": " + "" + flip.ToString().ToLower() + ",";
		text += "\n\"permanent\": " + "" + permanent.ToString().ToLower() + ","; 
		text += "\n\"cardID\": " + "\"" + cardID + "\","; 
		text += "\n\"polyCard\": " + "\"" + polyCard + "\","; 
		text += "\n\"glitchValue\": " + "\"" + glitchValue + "\",";  
		

		
	
		return text;
	}


}
