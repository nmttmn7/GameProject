using System;
using System.Collections;
using System.Collections.Generic;
using Godot;

using TheLiquidFire.AspectContainer;

public interface IAbilityLoader {
	void Load (IContainer game, Ability ability);
	string LoadText (Ability ability);


	public string InterpretTarget(Ability ability){
		string str = "";

		var ITargetSelector = ability.GetAspect<ITargetSelector>();

		if(ITargetSelector != null){
		str += ITargetSelector.LoadText(); 
		if(!ability.abilityCount.ToString().Equals("1"))
			str +=  "X" + ability.abilityCount + " ";
		}

		return str;

	}
	public string InterpretCondition(Ability ability){
		var condition = ability.GetAspect<Condition>();

		string text = "";

		if(condition == null)
			return text;
		
		string info = condition.conditionInfo;
		string valueInfo = condition.value;
		
	//	info = info.Replace("infoability", "Ability");
	//	info = info.Replace("infostatus", "Status");
		text += "IF " + InterpretData(info) + " " + condition.comparator + " " + InterpretData(valueInfo) + " ";
		return text;
		
	}

	
	public string InterpretStatus(Ability ability){

		StatusData statusData = ability.GetAspect<StatusData>();
		string description = "";

		if(statusData == null)
			return description;
		
		description += "Apply ";
	
		description += statusData.InterpretStatus(statusData.data);

		return description;
	}

	public string InterpretData(string sub)
    {
		String info = sub;
		info = info.Replace(" ", "");
		info = info.ToLower();
		if(info.Contains("(")){
			return "";
		}
		
		if (info.Contains("player"))
		{
			info = info.Replace("player", "");

			if(info.Contains("all"))
				return "[img=40]" + "res://Sprites/StatusIcons/hearts.png" + "[/img] ";

			if(info.Contains("discard"))
				return "[img=40]" + "res://Sprites/StatusIcons/card-burn.png" + "[/img] ";

			if(info.Contains("graveyard"))
				return "graveyard ";

			if(info.Contains("mana"))
				return "mana ";
			
			info = info.Replace("name", "");
			return "\"" + info.Capitalize() + "\" cards ";
		}

		if (info.Contains("card"))
		{
			

			if(info.Contains("currenthealth"))
				return "current health ";

			if(info.Contains("maxhealth"))
				return "max health ";

			if(info.Contains("missinghealth"))
				return "missing health ";

			if(info.Contains("status")){
				info = info.Replace("cardstatus","");
				var data = DeckFactory.Statuses[info.ToLower()];
				string statSprite = (string)data["sprite"];
				return "[img=40]" + statSprite + "[/img] ";
			}
			
		}

		if (info.Contains("target"))
		{
			

			if(info.Contains("currenthealth"))
				return "target's current health ";

			if(info.Contains("maxhealth"))
				return "target's max health ";

			if(info.Contains("missinghealth"))
				return "target's missing health ";

			if(info.Contains("status")){
				info = info.Replace("targetstatus","");
				var data = DeckFactory.Statuses[info.ToLower()];
				string statSprite = (string)data["sprite"];
				return "target's [img=40]" + statSprite + "[/img] ";
			}
			
		}

		if(info.Contains("abilitychainposition")){
			return "[img=40]" + "res://Sprites/UIIcons/armoured-shell.png" + "[/img] ";
		}
		
			return info + " ";
	}
	

}
