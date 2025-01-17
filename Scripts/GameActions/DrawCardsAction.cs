using System.Collections;
using System.Collections.Generic;
using TheLiquidFire.AspectContainer;

using System;
using Godot;

public class DrawCardsAction : GameAction, IAbilityLoader {
	public int amount;
	public List<Card> cards = new();
	public List<Card> targets;
	public Ability attachedAbility;
	public bool createdCard = false;
	#region Constructors
	public DrawCardsAction() {
		
	}

	

	public DrawCardsAction(Card target,int amount)
	{
		targets = new List<Card>(1);
		targets.Add(target);
		this.amount = amount;

	}

	public DrawCardsAction(List<Card> targets,int amount)
	{
		this.targets = targets;
		this.amount = amount;

	}
	#endregion

	#region IAbility
	public void Load (IContainer game, Ability ability) {
		targets = new List<Card>();
		attachedAbility = ability;
		var targetSelector = ability.GetAspect<ITargetSelector>();
		var cards = targetSelector.SelectTargets(game);
		targets.AddRange(cards);


		//player = game.GetMatch ().players [ability.card.ownerIndex];
		amount = Convert.ToInt32 (ability.GetInfo());
	/*	var availableSlots = Mathf.Clamp(Player.maxHand - player.hand.Count, 0, Player.maxHand);
		if(availableSlots == 0) 
		amount = 0;
		else if(availableSlots < amount) 
		amount = availableSlots; */
	}

    public string LoadText(Ability ability){
		IAbilityLoader IAbility = this as IAbilityLoader;
		string str = ability.GetInfo();
		string description = "";
		
		
		
		if(!str.Contains("skip")){
		description += "Draw ";
		var split = str.Split("|");
		foreach(var sub in split){
			description += IAbility.InterpretData(sub);
		}
	
		}

		if(description == "0 " && !description.Contains("|"))
			description = "";

		
		description += IAbility.InterpretTarget(ability);
		description += IAbility.InterpretCondition(ability);
		

		return description;
	}

    #endregion
}