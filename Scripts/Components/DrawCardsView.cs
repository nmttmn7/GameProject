using System.Collections;
using System.Collections.Generic;

using TheLiquidFire.Notifications;
using TheLiquidFire.AspectContainer;
using TheLiquidFire.Extensions;
using Godot;


public partial class DrawCardsView : Node {

	[Export] PackedScene cardConstruct;

	BoardView boardView;

	StatusView statusView;
	
	Node2D AudioManager;

	[Export]
	public Resource resource;
	RichTextLabel deckCardLabel;
	public override void _Ready()
	{
		this.AddObserver (OnPrepareDrawCards, Global.PrepareNotification<DrawCardsAction> ());

		this.AddObserver (OnPerformDrawUICards, Global.PerformNotification<DrawCardsAction> ());
		boardView = GetTree().Root.GetNode("Main").GetNode("GameViewSystem").GetNode<BoardView>("Board");
		statusView = GetTree().Root.GetNode("Main").GetNode("GameViewSystem").GetNode<StatusView>("StatusView");
		AudioManager = GetTree().Root.GetNode<Node2D>("AudioManager");
		deckCardLabel = GetTree().Root.GetNode("Main").GetNode("DrawConstruct").GetChild<RichTextLabel>(1);
	}

	public override void _ExitTree()
    {
		this.RemoveObserver (OnPrepareDrawCards, Global.PrepareNotification<DrawCardsAction> ());

		this.RemoveObserver (OnPerformDrawUICards, Global.PerformNotification<DrawCardsAction> ());
	}




void OnPerformDrawUICards(object sender, object args){
		var action = args as DrawCardsAction;
		Player player = action.player;
		if(player == null)
			return;
		if(player.index == 1)
			return;
			
		deckCardLabel.Text = action.player.deck.Count.ToString();
	}






	void OnPrepareDrawCards (object sender, object args) {
		var action = args as DrawCardsAction;
		action.perform.viewer = DrawCardsViewer;
		
	}

	
	IEnumerator DrawCardsViewer (IContainer game, GameAction action) {
		yield return true; // perform the action logic so that we know what cards have been drawn
		var drawAction = action as DrawCardsAction;

		var playerView = boardView.playerViews [drawAction.player.index];

		for (int i = 0; i < drawAction.cards.Count; ++i) {
			
			
			
			var instance = cardConstruct.Instantiate();

			if(drawAction.createdCard == true){
				instance.GetChild(0).GetParent<Node2D>().Visible = true;
			}
		//	instance.Reparent(playerView.hand.handPos, false);
			playerView.hand.handPos.AddChild(instance);
			
			
			CardView cardView = instance.GetChild<CardView>(0);
		
			AudioManager.Call("create_audio","CARD_DRAW");

			if(playerView.player.index == 0){
				cardView.cardNodePos.Position += new Vector2(0, 500f);
			}else
				cardView.cardNodePos.Position += new Vector2(0, -500f);
		//	cardView.cardNodePos.Position = playerView.hand.handPos.Position;
			//cardView.cardNodePos.Rotation = playerView.deck.topCard.Rotation;
		//	instance.Reparent(playerView.deck.topCard,true);
			cardView.card = drawAction.cards [i];
	
			
		
			var showPreview = action.player.mode == ControlModes.Local;
			
			if(AugmentSystem.CheckStatus(cardView.card, "power01")){
				GD.Print("WOW");
				cardView.button.Call("blinded");
			}
			statusView.GenerateAllStatuses(cardView);

			
			var addCard = playerView.hand.AddCard (cardView, drawAction.createdCard,drawAction.attachedAbility);
			while (addCard.MoveNext ())
				yield return null;


		}
	}
}