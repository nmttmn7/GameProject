using System.Collections;
using System.Collections.Generic;
using Godot;
using TheLiquidFire.AspectContainer;
using TheLiquidFire.Extensions;

public class EvokeTarget : Aspect, ITargetSelector {

	public List<Card> SelectTargets (IContainer game) {
		var card = (container as Ability).card;
		
		var status = (container as Ability).abilityRoot.container as Status;
		if(status != null){
			if(status.flip)
			card = (container as Ability).evokedAbility.card;
		}

		Ability ability = container as Ability;
		
		
		var result = ability.evokedAbility.GetAspect<ITargetSelector>().SelectTargets(game);
		
		
	
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
