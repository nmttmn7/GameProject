using System.Collections;
using System.Collections.Generic;
using Godot;
using TheLiquidFire.AspectContainer;
using TheLiquidFire.Notifications;


public partial class ChangeTurnView : Node{
	[Export] RichTextLabel richTextLabel;
	//[Export] ChangeTurnButtonView buttonView;
	[Export] Node changeTurnNode;
	IContainer game;

		public void ButtonPressed(Node Camera, InputEvent inputEvent, Vector3 position, Vector3 normal, int shapeIDX){
		if(inputEvent is InputEventMouseButton){
			InputEventMouseButton inputMouseEvent = (InputEventMouseButton)inputEvent;
			if(inputMouseEvent.IsPressed() == true && inputMouseEvent.ButtonIndex == MouseButton.Left){

				if (CanChangeTurn ()) {
			var system = game.GetAspect<MatchSystem> ();
			system.ChangeTurn ();
		} else {
			// TODO: Play an error input sound effect?
		}

			}
			}	
	}

public void ChangeTurnButtonPressed(){
	if (CanChangeTurn ()) {
			var system = game.GetAspect<MatchSystem> ();
			system.ChangeTurn ();
		} else {
			// TODO: Play an error input sound effect?
		}
}

	bool CanChangeTurn () {
		var stateMachine = game.GetAspect<StateMachine> ();
		if (!(stateMachine.currentState is PlayerIdleState))
			return false;

		var player = game.GetMatch ().CurrentPlayer;
		if (player.mode != ControlModes.Local)
			return false;

		return true;
	}

	public override void _Ready()
	{
	game = GetTree().Root.GetNode("Main").GetNode<GameViewSystem>("GameViewSystem").container;
	this.AddObserver (OnPrepareChangeTurn, Global.PrepareNotification<ChangeTurnAction> (), game);
	}


    public override void _ExitTree()
    {
		this.RemoveObserver(OnPrepareChangeTurn, Global.PrepareNotification<ChangeTurnAction> (), game);
	}

	void OnPrepareChangeTurn (object sender, object args) {
		var action = args as ChangeTurnAction;
		action.perform.viewer = ChangeTurnViewer;
	}

	IEnumerator ChangeTurnViewer (IContainer game, GameAction action) {
		var dataSystem = game.GetAspect<DataSystem> ();
		var changeTurnAction = action as ChangeTurnAction;
		var targetPlayer = dataSystem.match.players [changeTurnAction.targetPlayerIndex];
	
		var banner = ShowBanner (targetPlayer);
		var button = FlipButton (targetPlayer);
		var isAnimating = true;

		do {
			var bannerOn = banner.MoveNext ();
			var buttonOn = button.MoveNext ();
			isAnimating = bannerOn || buttonOn;
			yield return null;
		} while (isAnimating);
	}

	IEnumerator ShowBanner (Player targetPlayer) {
		if (targetPlayer.mode != ControlModes.Computer)
			yield break;


	
	
	Tween tween1 = CreateTween();
	tween1.TweenProperty(richTextLabel, "scale", new Vector2(2, 2), 0.25f);
	while (tween1.IsRunning()) { yield return null; }
//	var tweener = yourTurnBanner.ScaleTo(Vector3.One, 0.25f, EasingEquations.EaseOutBack);
	//	while (tweener.IsPlaying) { yield return null; }

	Tween tween2 = CreateTween();
	tween2.TweenInterval(1f);
	while(tween2.IsRunning()){ yield return null; }	
	//	tweener = yourTurnBanner.Wait (1f);
	//	while (tweener.IsPlaying) { yield return null; }

    Tween tween3 = CreateTween();
	tween3.TweenProperty(richTextLabel, "scale", Vector2.Zero, 0.25f);
	while(tween3.IsRunning()){ yield return null; }	
	//	tweener = yourTurnBanner.ScaleTo (Vector3.zero, 0.25f, EasingEquations.EaseInBack);
	//	while (tweener.IsPlaying) { yield return null; }*/
	
	}

	IEnumerator FlipButton (Player targetPlayer) {
		var up = new Vector2(0,0);
		var down = new Vector2(180,0);
		
		var targetRotation = targetPlayer.mode == ControlModes.Local ? up : down;
		Tween tween = CreateTween();
		tween.TweenInterval(0f);
	//	tween.TweenProperty(changeTurnNode, "rotation", targetRotation, 0.5f).SetTrans(Tween.TransitionType.Back);
		
		while (tween.IsRunning()) { yield return null; }
	}

}