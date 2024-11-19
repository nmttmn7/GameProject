using System.Collections;
using System.Collections.Generic;
using TheLiquidFire.AspectContainer;

using System;

public class DiscardCardsAction : GameAction, IAbilityLoader {
	
	public List<Card> cards;

	#region Constructors
	public DiscardCardsAction() {
		
	}

	public DiscardCardsAction(Player player, List<Card> cards) {
		this.player = player;
		this.cards = cards;
	}
	public DiscardCardsAction(Player player, Card card) {
		this.player = player;
		List<Card> list = new();
		list.Add(card);
		this.cards = list;
	}
	#endregion

	#region IAbility
	public void Load (IContainer game, Ability ability) {
		var targetSelector = ability.GetAspect<ITargetSelector> ();
		var selectedCards = targetSelector.SelectTargets (game);
		cards = new List<Card> ();
		foreach (Card card in selectedCards) {
			var selected = card as Card;
			if (selected != null)
				cards.Add (selected);
		}
	
	}

    public string LoadText(Ability ability)
    {
        throw new NotImplementedException();
    }
    #endregion



}