using System.Collections;
using System.Collections.Generic;

using TheLiquidFire.Notifications;
using TheLiquidFire.AspectContainer;
using TheLiquidFire.Extensions;
using Godot;


public partial class DrawCardsView : Node {

	
	AfflictionView afflictionView;
	
	Node2D AudioManager;

	[Export]
	public Resource resource;
	RichTextLabel deckCardLabel;
	
	public ViewView view;
	public override void _Ready()
	{
		this.AddObserver (OnPrepareDrawCards, Global.PrepareNotification<DrawCardsAction> ());

		this.AddObserver (OnPerformDrawUICards, Global.PerformNotification<DrawCardsAction> ());

		afflictionView = GetTree().Root.GetNode("Main").GetNode("GameViewSystem").GetNode<AfflictionView>("AfflictionView");
		view = GetTree().Root.GetNode("Main").GetNode("GameViewSystem").GetNode<ViewView>("ViewView");
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
		yield return true;
		var drawAction = action as DrawCardsAction;

		

		for (int i = 0; i < drawAction.cards.Count; ++i) {
			
			
			
			var instance = DataManager.node.cardConstruct.Instantiate();

			//if(drawAction.createdCard == true)

			instance.GetChild(0).GetParent<Node2D>().Visible = true;

			CardView cardView = instance.GetChild<CardView>(0);

			if(drawAction.cards[i].ownerIndex == 0){

				view.playerHand2D.AddChild(instance);
				cardView.cardNodePos.Position += new Vector2(0, 500f);

			}else{
				
				view.enemyHand2D.AddChild(instance);
				cardView.cardNodePos.Position += new Vector2(0, 0f);

			}
			

			
		
			AudioManager.Call("create_audio","CARD_DRAW");


			cardView.card = drawAction.cards [i];
			
	
			afflictionView.GenerateAllStatuses(cardView);

			
			var addCard = view.AddCard (cardView, drawAction.createdCard,drawAction.attachedAbility,cardView.card.ownerIndex);
			while (addCard.MoveNext ())
				yield return null;


		}
	}
}