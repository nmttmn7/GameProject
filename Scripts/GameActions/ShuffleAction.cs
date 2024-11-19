using System.Collections;
using System.Collections.Generic;
using TheLiquidFire.AspectContainer;
using Godot;
using System;

public class ShuffleAction : GameAction, IAbilityLoader
{
	public int amount;
	public List<Card> cards;
	#region Constructors
	public ShuffleAction()
	{

	}

	public ShuffleAction(Player player) {
		this.player = player;
		
	}
	#endregion

	#region IAbility
	public void Load(IContainer game, Ability ability)
	{
		player = game.GetMatch().players[ability.card.ownerIndex];
		amount = Convert.ToInt32(ability.userInfo);
	}

    public string LoadText(Ability ability)
    {
        throw new NotImplementedException();
    }
    #endregion
}