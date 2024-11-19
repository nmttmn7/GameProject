using System.Collections;
using System.Collections.Generic;
using Godot;
using TheLiquidFire.Notifications;
using TheLiquidFire.AspectContainer;
using TheLiquidFire.Extensions;

public partial class DiscardCardsView : Node
{

	BoardView boardView;
	Node playerHand;
	Node enemyHand;

	RichTextLabel discardCardLabel;
		public override void _Ready()
	{	

		this.AddObserver (OnDeathDiscardNotification, ActionSystem.deathReaperNotification);

		this.AddObserver(OnPrepareDiscardCards, Global.PrepareNotification<DiscardCardsAction>());

		this.AddObserver (OnPerformDiscardUICards, Global.PerformNotification<DiscardCardsAction> ());

		boardView = GetTree().Root.GetNode("Main").GetNode("GameViewSystem").GetNode<BoardView>("Board");
		playerHand = GetTree().Root.GetNode("Main").GetNode("PlayerHand");
		enemyHand = GetTree().Root.GetNode("Main").GetNode("EnemyHand");
		discardCardLabel = GetTree().Root.GetNode("Main").GetNode("DiscardConstruct").GetChild<RichTextLabel>(1);


	}
    public override void _ExitTree()
    {	
		this.RemoveObserver (OnDeathDiscardNotification, ActionSystem.deathReaperNotification);

		this.RemoveObserver(OnPrepareDiscardCards, Global.PrepareNotification<DiscardCardsAction>());

		this.RemoveObserver (OnPerformDiscardUICards, Global.PerformNotification<DiscardCardsAction> ());


		
	}



	void OnDeathDiscardNotification (object sender, object args) {
		var children = enemyHand.GetChildren();
		
		foreach (var child in children) {
			
			if(child.GetChild<CardView>(0).card.zone == Zones.Discard){
				
			}
		}
	}

	
void OnPerformDiscardUICards(object sender, object args){
		var action = args as DiscardCardsAction;
		Player player = action.player;
		if(player == null)
			return;
		if(player.index == 1)
			return;
			
		discardCardLabel.Text = action.player.discard.Count.ToString();
	}






	void OnPrepareDiscardCards(object sender, object args)
	{
		var action = args as DiscardCardsAction;
		action.perform.viewer = DiscardCardsViewer;
	}

	IEnumerator DiscardCardsViewer(IContainer game, GameAction action)
	{
		var discardAction = action as DiscardCardsAction;
		
		
		for (int i = 0; i < discardAction.cards.Count; ++i)
		{	
			
			var playerView = boardView.playerViews[discardAction.cards[i].ownerIndex];

			var cardView = playerView.hand.GetView(discardAction.cards[i]);
			
			var discardCard = playerView.hand.DiscardCard(cardView);
			while (discardCard.MoveNext())
				yield return null;
		}

		
	}


}