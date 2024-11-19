using System.Collections;
using System.Collections.Generic;

using TheLiquidFire.AspectContainer;
using TheLiquidFire.Extensions;

public class RandomTarget : Aspect, ITargetSelector {
	public Mark mark;
	
	bool spread = false;
	//spread picked random cards first then checks if condition is met, makes it so status should be spread to get most out of random
	//Not spread pick cards that meet the condition then picks a random card
	public List<Card> SelectTargets (IContainer game) {
		var result = new List<Card> ();
		var system = game.GetAspect<TargetSystem> ();
		var card = (container as Ability).card;
		var target = card.GetAspect<Target> ();

		var condition = (container as Ability).GetAspect<Condition> ();

		
				
		if(mark.alliance == Alliance.Target) {

			
			if(spread && condition != null){

			var marks = system.GetMarks (card, target.selected, null);

			if (marks.Count == 0)
			return result;
			
			var rand = marks.Random();
			if(condition.ConditionCheck(game,rand))
				result.Add (rand);
			

			return result;

			}else{


			var marks = system.GetMarks (card, target.selected, condition);

			if (marks.Count == 0)
			return result;
		
			result.Add (marks.Random ());
			

			return result;

			}

		}else{

			if(spread && condition != null){

			var marks = system.GetMarks (card, mark, null);

			if (marks.Count == 0)
			return result;
			
			var rand = marks.Random();
			if(condition.ConditionCheck(game,rand))
				result.Add (rand);
			

			return result;

			}else{

			var marks = system.GetMarks (card, mark, condition);

			if (marks.Count == 0)
			return result;
		
			result.Add (marks.Random ());


			return result;

			}
		}

		
		
	}

	public void Load(Dictionary<string, object> data) {
		var markData = (Dictionary<string, object>)data["mark"];
		mark = new Mark (markData);
		
	}

	public string LoadText(){
		string str = "";
		str += "to random " ;
		if(mark.alliance == Alliance.Target){
			str += "targets ";
		}else if(mark.alliance == Alliance.Ally)
			str += "allies ";
		else if(mark.alliance == Alliance.Enemy)
			str += "enemies ";
		else if(mark.alliance == Alliance.Any)
			str = "to anyone ";


		
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
