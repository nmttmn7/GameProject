using System;
using System.Collections;
using System.Collections.Generic;
using Godot;

using TheLiquidFire.AspectContainer;
using TheLiquidFire.Notifications;

public class Condition : TheLiquidFire.AspectContainer.Container, IAspect {
	public IContainer container { get; set; }
	public Ability ability { get { return container as Ability; } }
	public string conditionInfo {get; set;}
	public string comparator { get;set;}
	public string value { get;set;}
	
	
	public bool ConditionCheck(IContainer game, Card card){
		
	
		

		
			
			if(comparator.Equals("contain")){
				string name = card.name.ToLower();
				if(name.Contains(conditionInfo) || name.Contains("value"))
					return true;
			}

			int amount = game.GetAspect<AugmentSystem>().ParseAbilityInfo(conditionInfo, card, ability);
			
			int output = game.GetAspect<AugmentSystem>().ParseAbilityInfo(value, card, ability);

			//GD.Print("AMOUNT = " + amount +"\n" + "Output = " + output);

			if(comparator.Equals("=")){
				if(amount == output)
					return true;
			}else if(comparator.Equals("!=")){
				if(amount != output)
					return true;
			}else if(comparator.Equals("<=")){
				if(amount <= output)
					return true;
			}else if(comparator.Equals(">=")){
				if(amount >= output)
					return true;
			}else if(comparator.Equals("<")){
				if(amount < output)
					return true;
			}else if(comparator.Equals(">")){
				if(amount > output)
					return true;
			}

		
		
		return false;
	}

	public string Save(){
		string text = "";
		text += "\n\"condition\": {"; 
		
		text += "\n\"info\": " + "\"" + conditionInfo + "\","; 
		text += "\n\"comparator\": " + "\"" + comparator + "\","; 
		text += "\n\"value\": " + "\"" + value + "\","; 
		
		text += "\n}";
	
		return text;
	}
}