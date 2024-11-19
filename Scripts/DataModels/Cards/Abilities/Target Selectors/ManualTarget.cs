using System.Collections;
using System.Collections.Generic;
using Godot;
using TheLiquidFire.AspectContainer;
using TheLiquidFire.Extensions;

public class ManualTarget : Aspect, ITargetSelector {

	public List<Card> SelectTargets (IContainer game) {
		var card = (container as Ability).card;
		var target = card.GetAspect<Target> ();
		var result = new List<Card> ();
		var condition = (container as Ability).GetAspect<Condition> ();

		if(condition != null){
		if(condition.ConditionCheck(game, target.selected))
			result.Add (target.selected);
		}else
		result.Add (target.selected);
	
		return result;
	}

	public void Load(Dictionary<string, object> data) {
		
	}

	public string Save(){
		string text = "";
		text += "\n\"targetSelector\": {"; 
		text += "\n\"type\": " + "\"" + GetType() + "\","; 

		text += "\n}";
	
		return text;
	}

    public string LoadText()
    {
		return "";
    }

}
