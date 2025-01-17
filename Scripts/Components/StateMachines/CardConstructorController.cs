using System.Collections;
using System.Collections.Generic;
using Godot;
using TheLiquidFire.AspectContainer;
using TheLiquidFire.Notifications;

public partial class CardConstructorController : Node {

	//IContainer game;
	TheLiquidFire.AspectContainer.Container container;
	StateMachine stateMachine;
	CardView activeCardView;
	CardView targetCardView;
	[Export] DisplayObjectsView displayObjectsView;
	[Export] CardConstructorView cardConstructorView;

	private float centerPlayPoint = -350f;
    public override void _EnterTree()
    {
    	//game = GetTree().Root.GetNode("Main").GetNode<GameViewSystem>("GameViewSystem").container;
		//displayObjectsView = GetTree().Root.GetNode("Main").GetNode("GameViewSystem").GetNode<DisplayObjectsView>("DisplayObjectsView");
		container = new TheLiquidFire.AspectContainer.Container ();
		stateMachine = container.AddAspect<StateMachine> ();
		container.AddAspect (new WaitingForInputState ()).owner = this;
		
		container.AddAspect (new ConfirmCancelState ()).owner = this;

		container.AddAspect (new ConfirmState ()).owner = this;

		container.AddAspect (new ResetState ()).owner = this;

		container.AddAspect (new EndState ()).owner = this;

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
		public CardConstructorController owner;
	}

	private class WaitingForInputState : BaseControllerState, IClickableHandler {
		public void OnClickNotification (object sender, object args) {
		
			

			


			if(args.ToString() != "OnClick")
				return;

				var clickable = sender as Clickable;
				var cardView = clickable.GetParent().GetChildOrNull<CardView>(0);
				
				


	

			
			owner.activeCardView = cardView;
			cardView.button.Call("_on_button_down");
		//	owner.displayObjectsView.DeleteAllOtherObjects(clickable.GetParent().GetParent(), clickable.GetParent());
			owner.stateMachine.ChangeState<ConfirmCancelState> (); 
		}

	}

	private class ConfirmCancelState : BaseControllerState, IClickableHandler {
		public void OnClickNotification (object sender, object args) {
		
			var clickable = sender as Clickable;
				
			var cardView = clickable.GetParent().GetChildOrNull<CardView>(0);
			var button = clickable.GetParent().GetChildOrNull<Button>(1);
		
		
			
			if(args.ToString() == "OnEnter"){
				owner.targetCardView =  cardView;
			
				return;
			}

			if(args.ToString() != "OnRelease")
				return;
			
			
			if(owner.activeCardView != owner.targetCardView && owner.targetCardView != null)
				owner.stateMachine.ChangeState<ConfirmState> (); 
			else
				owner.stateMachine.ChangeState<ResetState> (); 

			

		}
	}

		private class ConfirmState : BaseControllerState {
		public override void Enter () {
			
			base.Enter ();
			owner.cardConstructorView.CombineCards(owner.targetCardView.card, owner.activeCardView);
			owner.stateMachine.ChangeState<ResetState> ();
		}
	}

	private class EndState : BaseControllerState {
		public override void Enter () {
			
			base.Enter ();
			Tween tween = SceneSwitcher.node.CreateTween();
			tween.TweenInterval(2);
			tween.TweenCallback(Callable.From(() => SceneSwitcher.node.SwitchScene("res://Scenes/MapScene.tscn")));
		}
	}

	private class ResetState : BaseControllerState {
		public override void Enter () {
		
			base.Enter ();
			owner.activeCardView.button.Call("_on_drop_card", true);
			owner.activeCardView = null;
			owner.targetCardView = null;
			owner.stateMachine.ChangeState<WaitingForInputState> ();
			
		}
	}

 #endregion
}