using System.Collections;
using System.Collections.Generic;
using TheLiquidFire.AspectContainer;

using System;
using Godot;

public class DrawCardsAction : GameAction, IAbilityLoader {
	public int amount;
	public List<Card> cards;
	public List<Card> findCard = new();
	public Ability attachedAbility;
	public bool createdCard = false;
	#region Constructors
	public DrawCardsAction() {
		
	}

	public DrawCardsAction(Player player, int amount) {
		this.player = player;
		this.amount = amount;
	}
	#endregion

	#region IAbility
	public void Load (IContainer game, Ability ability) {
		attachedAbility = ability;
		player = game.GetMatch ().players [ability.card.ownerIndex];
		amount = Convert.ToInt32 (ability.userInfo);
		var availableSlots = Mathf.Clamp(Player.maxHand - player.hand.Count, 0, Player.maxHand);
		if(availableSlots == 0) 
		amount = 0;
		else if(availableSlots < amount) 
		amount = availableSlots;
	}

    public string LoadText(Ability ability)
    {
      return "Not done";
	}
    #endregion
}