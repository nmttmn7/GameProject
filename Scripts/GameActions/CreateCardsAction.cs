using System.Collections;
using System.Collections.Generic;
using TheLiquidFire.AspectContainer;

using System;
using Godot;

public class CreateCardsAction : GameAction, IAbilityLoader {
	

	public string cardID;
	
	public bool drawCreatedCard = false;

	public List<Card> targets;
	public Ability attachedAbility;

	#region Constructors
	public CreateCardsAction() {
		
	}

	
	#endregion

	#region IAbility
	public void Load (IContainer game, Ability ability) {

		attachedAbility = ability;
	
		
		var split = ability.userInfo.ToString().Split("|");
		cardID = split[0];
		drawCreatedCard = System.Convert.ToBoolean(split[1]);
		
		var targetSelector = ability.GetAspect<ITargetSelector>();
		var cards = targetSelector.SelectTargets(game);
		
		targets = new List<Card>();
		targets = cards;

		
			
		
	
	}

    public string LoadText(Ability ability)
    {
		string str = "";

		var split = ability.userInfo.ToString().Split("|");
		cardID = split[0];
		drawCreatedCard = System.Convert.ToBoolean(split[1]);

		//Create CARDID // On enemy or ally pile. if
        

		if(drawCreatedCard)
			str += "Draw ";
		else
			str += "Create ";

		if(cardID == "target" || cardID == "self")
			str += "a copy of " + cardID + " ";
			else
		str += "a " + (string)DeckFactory.Cards[cardID]["name"] + " ";

		str += InterpretTarget(ability);
		str += InterpretCondition(ability);

		return str;
    }
	public string InterpretCondition(Ability ability){
		var condition = ability.GetAspect<Condition>();

		string text = "";

		if(condition == null)
			return text;
		
		string info = condition.conditionInfo;
		string valueInfo = condition.value;
		
		info = info.Replace("infoability", "Ability");
		info = info.Replace("infostatus", "Status");
		text += "with " + InterpretData(info) + " " + condition.comparator + " " + InterpretData(valueInfo) + " ";
		return text;
		
	}

		public string InterpretData(string sub)
    {
		String info = sub;
		info = info.Replace(" ", "");
		info = info.ToLower();

		
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
				var data = DeckFactory.Statuses[info.Capitalize()];
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
				var data = DeckFactory.Statuses[info.Capitalize()];
				string statSprite = (string)data["sprite"];
				return "target's [img=40]" + statSprite + "[/img] ";
			}
			
		}
		
			return info + " ";
	}
	
	public string InterpretTarget(Ability ability){
		string str = "";

		var ITargetSelector = ability.GetAspect<ITargetSelector>();

		if(ITargetSelector != null){
		
		if(ITargetSelector.ToString().Contains("self"))
			str += "on ";

		str += ITargetSelector.LoadText(); 
	//	if(ability.abilityCount > 1)
			str +=  "X" + ability.abilityCount + " ";
		}

		return str;

	}
	
    #endregion



}