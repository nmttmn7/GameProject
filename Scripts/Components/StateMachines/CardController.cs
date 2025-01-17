using System.Collections;
using System.Collections.Generic;
using Godot;
using TheLiquidFire.AspectContainer;
using TheLiquidFire.Notifications;

public partial class CardController : Node {

	IContainer game;
	TheLiquidFire.AspectContainer.Container container;
	StateMachine stateMachine;
	CardView activeCardView;
	CardView targetCardView;
	private DisplayObjectsView displayObjectsView;
	public ViewView view;
	private float centerPlayPoint = -350f;
    public override void _EnterTree()
    {
    	game = GetTree().Root.GetNode("Main").GetNode<GameViewSystem>("GameViewSystem").container;
		displayObjectsView = GetTree().Root.GetNode("Main").GetNode("GameViewSystem").GetNode<DisplayObjectsView>("DisplayObjectsView");
		view = GetTree().Root.GetNode("Main").GetNode("GameViewSystem").GetNode<ViewView>("ViewView");
		container = new TheLiquidFire.AspectContainer.Container ();
		stateMachine = container.AddAspect<StateMachine> ();
		container.AddAspect (new WaitingForInputState ()).owner = this;
	
		container.AddAspect (new ConfirmCancelState ()).owner = this;
		
		container.AddAspect (new CancellingState ()).owner = this;
		container.AddAspect (new ConfirmState ()).owner = this;
		container.AddAspect (new ShowTargetState ()).owner = this;
		container.AddAspect (new TargetState ()).owner = this;
		container.AddAspect (new ResetState ()).owner = this;
		stateMachine.ChangeState<WaitingForInputState> ();
	}

    public override void _Ready()
    {
		this.AddObserver (OnClickNotification, Clickable.ClickedNotification);
	}

	public override void _ExitTree() {
		this.RemoveObserver (OnClickNotification, Clickable.ClickedNotification);
	}

	void OnClickNotification (object sender, object args) {
		var handler = stateMachine.currentState as IClickableHandler;
		if (handler != null)
			handler.OnClickNotification (sender, args);
	}

	#region Controller States
	private interface IClickableHandler {
		void OnClickNotification (object sender, object args);
	}

	private abstract class BaseControllerState : BaseState {
		public CardController owner;
	}

	private class WaitingForInputState : BaseControllerState, IClickableHandler {
		public void OnClickNotification (object sender, object args) {
			
			
			var gameStateMachine = owner.game.GetAspect<StateMachine> ();

		//	if (!(gameStateMachine.currentState is PlayerIdleState))
		//		return;


			if(args.ToString() != "OnClick")
				return;

				var clickable = sender as Clickable;
				
				var cardView = clickable.GetParent().GetChildOrNull<CardView>(0);
				
			
				if (cardView == null || 
				cardView.card.zone != Zones.Hand || 
				cardView.card.ownerIndex != owner.game.GetMatch ().currentPlayerIndex){

				cardView.button.Call("_on_drop_card", true);
				return;
				}

			cardView.button.Call("_on_button_down");
			gameStateMachine.ChangeState<PlayerInputState> ();
			owner.activeCardView = cardView;
			owner.targetCardView = cardView;
			owner.stateMachine.ChangeState<ConfirmCancelState> (); 
		}
	}

	private class ConfirmCancelState : BaseControllerState, IClickableHandler {
		public void OnClickNotification (object sender, object args) {

			var clickable = sender as Clickable;
				
			var cardView = clickable.GetParent().GetChildOrNull<CardView>(0);
			var button = clickable.GetParent().GetChildOrNull<Button>(1);
		
			var target = owner.activeCardView.card.GetAspect<Target> ();
			
			if(args.ToString() == "OnEnter"){
				owner.targetCardView =  cardView;
			
				return;
			}

			if(args.ToString() == "OnRelease"){
				
			var gameStateMachine = owner.game.GetAspect<StateMachine> ();

			if (owner.game.GetAspect<ActionSystem> ().IsActive)
				owner.stateMachine.ChangeState<ResetState> (); 
			else if(owner.activeCardView != owner.targetCardView){

				owner.stateMachine.ChangeState<ConfirmState> (); 
				cardView.button.Set("following_mouse",false);

			}else{

					owner.stateMachine.ChangeState<ResetState> (); 
			}
				
				

				

			}
			
		}

    }


 private class CancellingState : BaseControllerState {
		public override void Enter () {
			base.Enter ();
			HideProcess().MoveNext();
			//owner.StartCoroutine (HideProcess ());
		}

		IEnumerator HideProcess () {
			
			//var handView = owner.activeCardView.GetComponentInParent<HandView> ();
			
			
			//yield return owner.StartCoroutine (handView.LayoutCards (true));
			owner.stateMachine.ChangeState<ResetState> ();
			yield return owner.view.displayObjectsView.LayoutObjects(owner.view.playerHand2D,99,true).MoveNext();
		}
	}

	private class ConfirmState : BaseControllerState {
		public override void Enter () {
			
			base.Enter ();
			var target = owner.activeCardView.card.GetAspect<Target> ();
			target.selected = owner.targetCardView.card;
			var action = new PlayCardAction (owner.activeCardView.card);
			owner.game.Perform (action);
			owner.stateMachine.ChangeState<ResetState> ();
		}
	}

	private class ShowTargetState : BaseControllerState {
		public override void Enter () {
			base.Enter ();
			HideProcess().MoveNext();
			//owner.StartCoroutine (HideProcess ());
		}

		IEnumerator HideProcess () {
	
			//var handView = owner.activeCardView.GetComponentInParent<HandView> ();
			
			//yield return owner.StartCoroutine (handView.LayoutCards (true));
			owner.stateMachine.ChangeState<TargetState> ();
			yield return owner.view.displayObjectsView.LayoutObjects(owner.view.playerHand2D,99,true).MoveNext();
		}
	}

	private class TargetState : BaseControllerState, IClickableHandler {
		public void OnClickNotification (object sender, object args) {
			var target = owner.activeCardView.card.GetAspect<Target> ();
			var cardView = (sender as Clickable).GetParent().GetChildOrNull<CardView>(0);
	
			if (cardView != null) {
				
				target.selected = cardView.card;
				owner.stateMachine.ChangeState<ConfirmState> ();
			} else {
			
				target.selected = null;
				owner.stateMachine.ChangeState<CancellingState> ();
			}
			
			
		}

     
    }

	private class ResetState : BaseControllerState {
		public override void Enter () {
			base.Enter ();
			owner.activeCardView.button.Call("_on_drop_card", true);
			owner.activeCardView = null;
			owner.targetCardView = null;
			owner.stateMachine.ChangeState<WaitingForInputState> ();
			if (!owner.game.GetAspect<ActionSystem> ().IsActive)
				owner.game.ChangeState<PlayerIdleState> ();
		}
	}
 #endregion
}