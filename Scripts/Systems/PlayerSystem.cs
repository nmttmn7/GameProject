using System.Collections;
using System.Collections.Generic;
using Godot;
using TheLiquidFire.AspectContainer;
using TheLiquidFire.Notifications;
using TheLiquidFire.Extensions;
using System;

public class PlayerSystem : Aspect, IObserve {

	public const string ValueChangedNotification = "PlayerSystem.ValueChangedNotification";
	public static int round = 1;

	public void Awake () {
		round = 1;
		this.AddObserver (OnPerformChangeTurn, Global.PerformNotification<ChangeTurnAction> (), container);
		this.AddObserver (OnPerformDrawCards, Global.PerformNotification<DrawCardsAction> (), container);

		

		this.AddObserver(OnPerformDiscardCards, Global.PerformNotification<DiscardCardsAction>(), container);
		this.AddObserver(OnPerformShuffle, Global.PerformNotification<ShuffleAction>(), container);

	//	this.AddObserver (OnValidatePlayCard, Global.ValidateNotification<DrawCardsAction> ());
	}

	public void Destroy () {
		this.RemoveObserver (OnPerformChangeTurn, Global.PerformNotification<ChangeTurnAction> (), container);
		this.RemoveObserver (OnPerformDrawCards, Global.PerformNotification<DrawCardsAction> (), container);
		
		//this.RemoveObserver (OnPerformPlayCard, Global.PerformNotification<PlayCardAction> (), container);
		this.RemoveObserver(OnPerformDiscardCards, Global.PerformNotification<DiscardCardsAction>(), container);
		this.RemoveObserver(OnPerformShuffle, Global.PerformNotification<ShuffleAction>(), container);

	//	this.AddObserver (OnValidatePlayCard, Global.ValidateNotification<DrawCardsAction> ());
	}

	void OnPerformChangeTurn (object sender, object args) {
		var action = args as ChangeTurnAction;
		var match = container.GetAspect<DataSystem> ().match;
		var player = match.players [action.targetPlayerIndex];
		
		if(player.mode == ControlModes.Local){
			
			List<Card> currentCards = match.CurrentPlayer[Zones.Hand];
			List<Card> opposingCards = match.OpponentPlayer[Zones.Hand];
			DiscardCards(match.CurrentPlayer, currentCards);
			DiscardCards(match.OpponentPlayer, opposingCards);
		DrawCards (match.CurrentPlayer, 4);
		DrawCards (match.OpponentPlayer, match.OpponentPlayer.DrawAmount());
		this.PostNotification (ValueChangedNotification, match.OpponentPlayer.drawList[match.OpponentPlayer.drawI]);
		round++;
		}
		
	}

	
	
	#region DiscardMethods

	void DiscardCards(Player player, List<Card> cards)
	{	
		var action = new DiscardCardsAction(player, cards);
		container.AddReaction(action);
	}

	void OnPerformDiscardCards(object sender, object args)
	{
		var action = args as DiscardCardsAction;

		action.cards = action.cards.ReturnNew();
		foreach (Card card in action.cards){
			ChangeZone(card, Zones.Discard);


		}
	}

	#endregion

	void OnPerformDrawCards (object sender, object args) {
		var action = args as DrawCardsAction;

		foreach (Card target in action.targets){
			Player player = container.GetMatch ().players [target.ownerIndex];


			foreach (Card card in action.cards)
			ChangeZone (card, Zones.Hand);

		var remain = action.amount - action.cards.Count;

		if(remain > 0){
			var shuffle = player [Zones.Discard].ReturnNew();
		
			foreach (Card card in shuffle)
				ChangeZone (card, Zones.Deck);

			List<Card> r = player [Zones.Deck].Draw(remain);
		
			foreach (Card card in r)
				ChangeZone (card, Zones.Hand);

			action.cards.AddRange(r);
		}

		}
		
			
		/*
		int deckCount = action.player [Zones.Deck].Count;
		
		int roomInHand = Player.maxHand - action.player [Zones.Hand].Count;
		
		int remain = 0;

		int drawCount = 4;

 
		if(deckCount >= action.amount){
		
		drawCount = action.amount;

		action.cards = action.player [Zones.Deck].Draw(drawCount);
		
		foreach (Card card in action.cards)
			ChangeZone (card, Zones.Hand);
		

		}else
		{
			
			remain = action.amount - deckCount;

			action.cards = action.player [Zones.Deck].Draw(drawCount);
		
			foreach (Card card in action.cards)
				ChangeZone (card, Zones.Hand);

			var shuffle = action.player [Zones.Discard].ReturnNew();
		
			foreach (Card card in shuffle)
				ChangeZone (card, Zones.Deck);

			List<Card> r = action.player [Zones.Deck].Draw(remain);
		
			foreach (Card card in r)
				ChangeZone (card, Zones.Hand);

			action.cards.AddRange(r);

		} */


		

		
	}

	





	void DrawCards (Player player, int amount) {
		var action = new DrawCardsAction (player.GetACard(), amount);
		container.AddReaction (action);
	}



	void ChangeZone (Card card, Zones zone, Player toPlayer = null) {
		var cardSystem = container.GetAspect<CardSystem> ();
		cardSystem.ChangeZone (card, zone, toPlayer);
	}

	void Shuffle(Player player)
	{
		var action = new ShuffleAction(player);
		container.AddReaction(action);
	}
	void OnPerformShuffle(object sender, object args)
	{
		var action = args as ShuffleAction;
		action.cards = action.player[Zones.Discard].Draw(action.player[Zones.Discard].Count);
		foreach (Card card in action.cards)
			ChangeZone(card, Zones.Deck);
	}

	
	
}