using System.Collections;
using System.Collections.Generic;

using TheLiquidFire.Notifications;
using TheLiquidFire.AspectContainer;
using Godot;
using System.Numerics;
using System.Linq;

public partial class HandView : Node {
	[Export] public Node2D deckPos;
	[Export] public Node2D handPos;
	[Export] public HandView enemyHandView;
	
	[Export] public PackedScene animationShake;
	private Node2D playerHand2D;
	private Node2D enemyHand2D;
	private Camera2D camera2D;
 	public DisplayObjectsView displayObjectsView;
	public override void _Ready()
	{	
		playerHand2D = GetTree().Root.GetNode("Main").GetNode<Node2D>("PlayerHand");
		enemyHand2D = GetTree().Root.GetNode("Main").GetNode<Node2D>("EnemyHand");
		camera2D = GetTree().Root.GetNode("Main").GetNode<Camera2D>("Camera2D");
	//	this.AddObserver (OnValidatePlayCard, Global.ValidateNotification<PlayCardAction> ());
		displayObjectsView = GetTree().Root.GetNode("Main").GetNode("GameViewSystem").GetNode<DisplayObjectsView>("DisplayObjectsView");
		
		this.AddObserver(OnPrepareDeath, Global.PrepareNotification<DeathAction>());
	
		this.AddObserver (OnValidateDamage, Global.ValidateNotification<DamageAction> ());

		this.AddObserver (OnValidateHeal, Global.ValidateNotification<HealAction> ());

		this.AddObserver (OnValidateCreateCard, Global.ValidateNotification<CreateCardsAction> ());

		

		//this.AddObserver (OnDiscardNotification, ActionSystem.deathReaperNotification);

	//	this.AddObserver (OnCompleteAllActions, ActionSystem.completeNotification);

	//	this.AddObserver (OnPolymorphZeroNotification, AugmentSystem.RemovePolymorphNotification);
	}

	public override void _ExitTree()
	{	
		
	//	this.RemoveObserver (OnValidatePlayCard, Global.ValidateNotification<PlayCardAction> ());

		this.RemoveObserver(OnPrepareDeath, Global.PrepareNotification<DeathAction>());
		
		this.RemoveObserver (OnValidateDamage, Global.ValidateNotification<DamageAction> ());

		this.RemoveObserver (OnValidateHeal, Global.ValidateNotification<HealAction> ());

		this.RemoveObserver (OnValidateCreateCard, Global.ValidateNotification<CreateCardsAction> ());
		
		;
		//this.RemoveObserver (OnDiscardNotification, ActionSystem.deathReaperNotification);
//		this.RemoveObserver (OnCompleteAllActions, ActionSystem.completeNotification);

	//	this.RemoveObserver (OnPolymorphZeroNotification, AugmentSystem.RemovePolymorphNotification);
	}
	
	#region Polymorph Methods

	void OnPolymorphZeroNotification (object sender, object args) {
		
		var notification = args as StatusNotification;
		var card = notification.card;
		
		CardView cardView = GetView(card);
		cardView.UpdateText();
		
	}


	#endregion
	
	#region Discard Methods
	public IEnumerator DiscardCard(CardView cardView)
	{
		//if DiscardAction is played twice on same card it will throw an error this code stops it
		if(cardView == null)
			yield break;

	
		var layout = DiscardAnimation(cardView);
		while (layout.MoveNext ())
				yield return null;
 
		
	
		yield return null;
	}

	public IEnumerator DiscardAnimation(CardView cardView)
	{
		
		Tween tween = CreateTween().SetParallel(false);
		
		if(cardView.card.ownerIndex == 0){
		tween.TweenProperty(cardView.button, "position",GetFactoredPostion(new Godot.Vector2(cardView.cardNodePos.Position.X, cardView.cardNodePos.GetParent<Node2D>().Position.Y), 
			new Godot.Vector2(0, 600),new Godot.Vector2(-125,-175) ),0.5f);
		}else{
		tween.TweenProperty(cardView.button, "position",GetFactoredPostion(new Godot.Vector2(cardView.cardNodePos.Position.X, cardView.cardNodePos.GetParent<Node2D>().Position.Y), 
			new Godot.Vector2(0, -600),new Godot.Vector2(-125,-175) ),0.5f);
		}

		while (tween != null && tween.IsRunning())
			yield return null;

		cardView.button.Call("callFree");

	
		
	
		cardView.GetParent().Reparent(cardView.GetParent().GetParent().GetParent());
		
		var layout = displayObjectsView.LayoutObjects(handPos,99);

			while (layout.MoveNext ())
				yield return null;

		
		
		yield return null;





	}
	
	#endregion

	

	void OnValidateHeal (object sender, object args) {

		var action = sender as HealAction;
		action.perform.viewer = OnPerformHealView;

	}

	[Export] PackedScene healAnimationConstruct;

	IEnumerator OnPerformHealView (IContainer game, GameAction action) {
		var healAction = action as HealAction;
		
		yield return true;

		
		var cardView = GetView(healAction.attachedAbility.card);

		var instance = healAnimationConstruct.Instantiate();
		cardView.AddChild(instance);

		SceneSwitcher.AudioManager.Call("create_audio","CARD_HEAL");
		foreach(var target in healAction.targets){
			var view = GetView((Card)target);
			if(view != null)
			view.UpdateText();
		}
		
		

		}

	

	void OnValidateDamage (object sender, object args) {
		var action = sender as DamageAction;
		action.perform.viewer = OnPerformDamageView;
	}

	void OnValidateCreateCard (object sender, object args) {
		var action = sender as CreateCardsAction;
		action.perform.viewer = OnPerformTransformView;
	}
	IEnumerator OnPerformTransformView (IContainer game, GameAction action) {
		var transformAction = action as CreateCardsAction;
		var attackType = transformAction.attachedAbility.GetAspect<ITargetSelector>();
		
		

		var attackCard = transformAction.attachedAbility.card;
		

		if (attackCard == null)
			yield break;
		
		
		var attackerView = GetView(attackCard);
		if(attackerView == null)
			yield break;
			
		

		

		attackerView.button.Call("_on_drop_card",false);
	
	

		attackerView.button.Call("_on_drop_card",true);
	}
    List<CardView> GetAllTargetsView(List<IDestructable> targets)
    {	
		List<CardView> targetViews = new();
		foreach(Card target in targets){
			var targetView = GetView(target);
		}	
		return targetViews;
	}

    Godot.Vector2 GetFactoredPostion(Godot.Vector2 origin, Godot.Vector2 endPosition, Godot.Vector2 offSet){
			var differenceX = endPosition.X - origin.X;
			var differenceY = endPosition.Y - origin.Y;
			var difference = new Godot.Vector2 (differenceX, differenceY);
			difference += offSet;
			return difference;
	}
	IEnumerator OnPerformDamageView (IContainer game, GameAction action) {
		var attackAction = action as DamageAction;
		var attackType = attackAction.attachedAbility.GetAspect<ITargetSelector>();
		
	
		
		var attackCard = attackAction.attachedAbility.card;
		

		if (attackCard == null || attackAction.targets == null)
			yield break;
		

		var attackerView = GetView(attackCard);
		if(attackerView == null){
			yield return true;

			foreach(var target in attackAction.targets){
			var view = GetView((Card)target);
			if(view != null)
			view.UpdateText();
		}
		yield break;
		}
			

		
		attackerView.button.Call("_on_drop_card",false);
	
		if(attackAction.targets.Count <= 0){
		
			yield return true;
			
		}else{
		var targetView = GetView((Card)attackAction.targets[0]);
		if(targetView == null)
			yield break;	
		
		Tween tween = CreateTween().SetParallel(false);
		
		var attackButton = attackerView.button;
		
		

		if(attackType.ToString() == "ManualTarget" || attackType.ToString() == "RandomTarget"){


			tween.TweenProperty(attackButton, "position",new Godot.Vector2(-125,-175),0.5f);

			tween.TweenProperty(attackButton, "position",GetFactoredPostion(new Godot.Vector2(attackerView.cardNodePos.Position.X, attackerView.cardNodePos.GetParent<Node2D>().Position.Y), 
			new Godot.Vector2(targetView.cardNodePos.Position.X, targetView.cardNodePos.GetParent<Node2D>().Position.Y),new Godot.Vector2(-125,-175) ),0.5f).SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.In);
			
			}else if(attackType.ToString() == "SelfTarget"){

			tween.TweenInterval(.1);

			}else{
			
			tween.TweenProperty(attackButton, "position",new Godot.Vector2(-125,-175),0.5f);

			var allEnemyChildren = enemyHand2D.GetChildren();
			var firstEnemy = allEnemyChildren.First();
			var lastEnemy = allEnemyChildren.Last();

			tween.TweenProperty(attackButton, "position",GetFactoredPostion(new Godot.Vector2(attackerView.cardNodePos.Position.X, attackerView.cardNodePos.GetParent<Node2D>().Position.Y), 
			firstEnemy.GetChild<CardView>(0).cardNodePos.Position,new Godot.Vector2(-125,-275) ),0.5f).SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.Out);

			tween.TweenProperty(attackButton, "position",GetFactoredPostion(new Godot.Vector2(attackerView.cardNodePos.Position.X, attackerView.cardNodePos.GetParent<Node2D>().Position.Y), 
			lastEnemy.GetChild<CardView>(0).cardNodePos.Position,new Godot.Vector2(-125,-275) ),0.3f).SetTrans(Tween.TransitionType.Circ).SetEase(Tween.EaseType.InOut);
		}
		

		while (tween != null && tween.IsRunning())
			yield return false;	
	
		Node instance;
		
		// Apply the attack damage here
		foreach(var target in attackAction.targets){
			var targetV = GetView((Card)target);
			if(targetV != null){
			targetV.button.Call("callShine");
			instance = animationShake.Instantiate();
			targetV.button.AddChild(instance);
			}
		}
		
		instance = animationShake.Instantiate();
		AnimationShake cameraShake= (AnimationShake)instance;
		camera2D.AddChild(cameraShake);
		
		yield return true;

		
		foreach(var target in attackAction.targets){
			var view = GetView((Card)target);
			if(view != null)
			view.UpdateText();
		}
		
		

		}

		attackerView.button.Call("_on_drop_card",true);
	}

	
	public IEnumerator AddCard (CardView card, bool showPreview, Ability attachedAbility) {
		
			if(attachedAbility != null){

			}
			
			card.UpdateText();
			var layout = displayObjectsView.LayoutObjects(handPos,99);
			while (layout.MoveNext ())
				yield return null;

			
	}

	


	

	public CardView GetView(Card card)
	{
		
		var children = playerHand2D.GetChildren();
		CardView cardView;
		foreach (Node childCard in children)
		{
			 cardView = childCard.GetChild<CardView>(0);
			if (cardView.card == card)
			{
				return cardView;
			}
		}
		var enemyChildren = enemyHand2D.GetChildren();
		
		foreach(Node enemyCard in enemyChildren){
			 cardView = enemyCard.GetChild<CardView>(0);
			if (cardView.card == card)
			{
				return cardView;
			}
		} 
		return null;
	}

	public void Dismiss(Node2D card)
	{
		
		
		card.QueueFree();
	

	}
	



	#region Death Card Methods

	public IEnumerator DeathAnimation(CardView cardView)
	{

		cardView.button.Call("callFree");

		//var hand = cardView.GetParent().GetParent();
		//hand.RemoveChild(cardView.GetParent());
		
	
		cardView.GetParent().Reparent(cardView.GetParent().GetParent().GetParent());
		
		var layout = displayObjectsView.LayoutObjects(handPos,99);
			while (layout.MoveNext ())
				yield return null;

		//tween.TweenCallback(Callable.From(() => Dismiss(cardView.cardNodePos)));
		
		yield return null;





	}
	void OnPrepareDeath(object sender, object args)
	{
		var action = args as DeathAction;
		var companion = action.card as Unit;
		if (companion != null)
		{
			if (GetParent<PlayerView>().player.index == action.card.ownerIndex)
				action.perform.viewer = DeathCard;
		}
	}

	IEnumerator DeathCard(IContainer game, GameAction action)
	{
		
		var playAction = action as DeathAction;
		CardView cardView = GetView(playAction.card);


		if (cardView == null)
			yield break;

	
		displayObjectsView.LayoutObjects(handPos, 99);
		var discard = DeathAnimation(cardView);
		while (discard.MoveNext())
			yield return null;


	}


    #endregion
}