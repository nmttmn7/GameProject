using System.Collections;
using System.Collections.Generic;

using TheLiquidFire.AspectContainer;
using System;
using Godot;
using TheLiquidFire.Extensions;

public class DamageAction : GameAction, IAbilityLoader
{
	public List<IDestructable> targets;
	public Ability attachedAbility;

	#region Constructors
	public DamageAction()
	{

	}

	public DamageAction(IDestructable target)
	{
		targets = new List<IDestructable>(1);
		targets.Add(target);

	}

	public DamageAction(List<IDestructable> targets)
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
		
		targets = new List<IDestructable>();
		foreach (Card card in cards)
		{
			var destructable = card as IDestructable;
			if (destructable != null)
				targets.Add(destructable);
		}
		var status = ability.abilityRoot.container as Status;
		if(status == null && !ability.userInfo.ToString().ToLower().Contains("(cardstatusrage)"))
			ability.userInfo += "|(cardstatusrage)";

	

	}	

	#endregion




	public string LoadText(Ability ability){
		IAbilityLoader IAbility = this as IAbilityLoader;
		string str = ability.userInfo.ToString();
		string description = "";
		
		
		
		if(!str.Contains("skip")){
		description += "Deal ";
		var split = str.Split("|");
		foreach(var sub in split){
			description += IAbility.InterpretData(sub);
		}

		if(description.Contains("img"))
		description += "as ";
		description += "damage ";
		
		}

		if(description == "0 damage " && !description.Contains("|"))
			description = "";

		description += IAbility.InterpretStatus(ability);
		description += IAbility.InterpretTarget(ability);
		description += IAbility.InterpretCondition(ability);
		

		return description;
	}

}