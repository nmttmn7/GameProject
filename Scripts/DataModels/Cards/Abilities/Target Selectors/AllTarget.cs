using System;
using System.Collections;
using System.Collections.Generic;
using Godot;

using TheLiquidFire.AspectContainer;
using TheLiquidFire.Extensions;

public class AllTarget : Aspect, ITargetSelector {
	public Mark mark;

	
	

	public List<Card> SelectTargets (IContainer game) {
		var result = new List<Card> ();
		
		var system = game.GetAspect<TargetSystem> ();
		var card = (container as Ability).card;

		var status = (container as Ability).abilityRoot.container as Status;
		if(status != null){
			if(status.flip)
			card = (container as Ability).evokedAbility.card;
		}

		
		Condition condition = (container as Ability).GetAspect<Condition>();
		
		if(mark.alliance == Alliance.Target) {
			
		
			var target = card.GetAspect<Target> ();

			var marks = system.GetMarks (card, target.selected, condition);

		//	marks = CertainCards(marks, game);

			result.AddRange (marks);
		
			return result;

		}else{
		
			
			var marks = system.GetMarks (card, mark, condition);

		//	marks = CertainCards(marks, game);

			result.AddRange (marks);
		
			return result;
		}
		
		
	}

   

    public void Load(Dictionary<string, object> data) {
		var markData = (Dictionary<string, object>)data["mark"];
		mark = new Mark (markData);

	//	cond = DeckFactory.CreateCondition(container as Ability, data);
		
	}

	public string LoadText(){
		string str = "";
		str += "to ALL " ;
		if(mark.alliance == Alliance.Target){
			str += "targets ";
		}else if(mark.alliance == Alliance.Ally)
			str += "allies ";
		else if(mark.alliance == Alliance.Enemy)
			str += "enemies ";
		else if(mark.alliance == Alliance.Any)
			str = "to EVERYONE ";


		
		return str;
	}


	public string Save(){
		string text = "";
		text += "\n\"targetSelector\": {"; 
		text += "\n\"type\": " + "\"" + GetType() + "\","; 
		text += "\n\"mark\": {"; 
		text += "\n\"alliance\": " + "\"" + mark.alliance + "\",";
		text += "\n\"zone\": " + "\"" + mark.zones + "\"";
		text += "\n}";
		text += "\n}";
	
		return text;
	}
}
