using System.Collections;
using System.Collections.Generic;

using TheLiquidFire.AspectContainer;
using System;
using Godot;
using TheLiquidFire.Extensions;

public class HealAction : GameAction, IAbilityLoader
{
	public List<Card> targets;
	public Ability attachedAbility;

	#region Constructors
	public HealAction()
	{

	}

	public HealAction(Card target)
	{
		targets = new List<Card>(1);
		targets.Add(target);

	}

	public HealAction(List<Card> targets)
	{
		this.targets = targets;

	}
	#endregion

	#region IAbility
	public void Load(IContainer game, Ability ability)
	{
		attachedAbility = ability;
		var targetSelector = ability.GetAspect<ITargetSelector>();
		var cards = targetSelector.SelectTargets(game);
		targets = new List<Card>();
		foreach (Card card in cards)
		{
			
				targets.Add(card);
		}




	}

	#endregion





	public string LoadText(Ability ability){
		IAbilityLoader IAbility = this as IAbilityLoader;
		string str = ability.GetInfo();
		string description = "";
		
		
		
		if(!str.Contains("skip")){

		description += "Heal ";

		var split = str.Split("|");
		foreach(var sub in split){
			description += IAbility.InterpretData(sub);
		}

		//if(str.Contains("info")){
		//	description += "status info ";
		//}

		
		
		}

		if(description == "Heal 0 " && !description.Contains("|"))
			description = "";

		description += IAbility.InterpretStatus(ability);
		description += IAbility.InterpretTarget(ability);
		description += IAbility.InterpretCondition(ability);
		

		return description;
	}


}