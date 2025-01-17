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
	private List<Card> connectedCards = new();

	public void AddConnectedCard(Card card){
		connectedCards.Add(card);
	}

	public List<Card> GetConnectedCards(){
		return connectedCards;
	}	
    public virtual void Load(Dictionary<string, object> data, Card target, Ability castedAbility, StatusSystem statusSystem = null) {
		
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


	
	
		if(data.ContainsKey("decr"))
			decrease += Int32.Parse((string)data ["decr"]);
			
		
		if(data.ContainsKey("value")){
			value = statusSystem.ParseAbilityInfo((string)data ["value"], target, castedAbility);
		}

		


		

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

	public virtual void ChangeValue(string data, string modifier, Card target, Ability castedAbility, StatusSystem statusSystem){
			
			string info =  data.ToString();
			info = CheckToken(info,castedAbility);


			int amount = statusSystem.ParseAbilityInfo(data, target, castedAbility);

			if(target.GetAspect<Afflictions>().GetStatusINT("curse") > 0) amount *= 2;

			if(modifier.Contains("+"))
				    value += amount;
			else if(modifier.Contains("-"))
				    value -= amount;
			else if(modifier.Contains("*"))
					value *= amount;	

			if(value < 0)
				value = 0;

			
	}

	public virtual string CheckToken(string data, Ability castedAbility){
		
		var token = castedAbility.card.GetAspect<Afflictions>().GetStatus("token") as Token;
		
		if(token != null){
			GD.Print("TOKEN BUILER ");
		return token.GetToken();
		}
		return data;
	}


	public virtual void Override(Afflictions afflictions, string id, Dictionary<string, object> data){

		if(!data.ContainsKey("override"))
			return;
			
			string oRide = (string)data["override"];
			var overrideData = oRide.Split(',');

			foreach(var parsedData in overrideData){
			afflictions.ReplaceStatus(parsedData, this);
			id = parsedData;
			}
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
