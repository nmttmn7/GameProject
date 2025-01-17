using System.Collections;
using System.Collections.Generic;

using TheLiquidFire.Notifications;
using TheLiquidFire.AspectContainer;
using Godot;
using System.Numerics;
using System.Linq;

public partial class ViewView : Node {
	
 	public Node2D playerHand2D;
	public Node2D enemyHand2D;
	
	
	
	
	private Camera2D camera2D;
	[Export]
 	public DisplayObjectsView displayObjectsView;

	public Dictionary <Card, CardView> hand = new();

	public List<CardView> GetCardView(List<Card> cardList){
		List<CardView> l = new();
		foreach (Card card in cardList)
			if(hand.TryGetValue(card, out CardView cardView))
				l.Add(cardView);

		return l;
	}

	public CardView GetCardView(Card card)
	{
		
			if(hand.TryGetValue(card, out CardView cardView)) return cardView;

			return null;
	}
	public override void _Ready()
	{	
		playerHand2D = GetTree().Root.GetNode("Main").GetNode<Node2D>("PlayerHand");
		enemyHand2D = GetTree().Root.GetNode("Main").GetNode<Node2D>("EnemyHand");
		camera2D = GetTree().Root.GetNode("Main").GetNode<Camera2D>("Camera2D");
	//	this.AddObserver (OnValidatePlayCard, Global.ValidateNotification<PlayCardAction> ());
		
		
		this.AddObserver(OnPrepareDeath, Global.PrepareNotification<DeathAction>());
	
		this.AddObserver (OnValidateDamage, Global.ValidateNotification<DamageAction> ());

		this.AddObserver (OnValidateHeal, Global.ValidateNotification<HealAction> ());

		this.AddObserver (OnValidateCreateCard, Global.ValidateNotification<CreateCardsAction> ());

		this.AddObserver (OnValidateTransform, Global.ValidateNotification<TransformCardsAction> ());
		

	}

	public override void _ExitTree()
	{	
		

		this.RemoveObserver(OnPrepareDeath, Global.PrepareNotification<DeathAction>());
		
		this.RemoveObserver (OnValidateDamage, Global.ValidateNotification<DamageAction> ());

		this.RemoveObserver (OnValidateHeal, Global.ValidateNotification<HealAction> ());

		this.RemoveObserver (OnValidateCreateCard, Global.ValidateNotification<CreateCardsAction> ());
		
		this.RemoveObserver (OnValidateTransform, Global.ValidateNotification<TransformCardsAction> ());


	}
	
	void OnValidateTransform (object sender, object args) {
		var action = sender as TransformCardsAction;
		action.perform.viewer = OnPerformTransformView;
	}

	IEnumerator OnPerformTransformView (IContainer game, GameAction action) {
		
		var transformAction = action as TransformCardsAction;

		yield return true;

		foreach(var target in transformAction.targets){
			var view = GetCardView((Card)target);		
			if(view != null)
			view.UpdateText();
		}

		


	}


	#region Polymorph Methods

	void OnPolymorphZeroNotification (object sender, object args) {
		
		var notification = args as StatusNotification;
		var card = notification.card;
		
		CardView cardView = GetCardView(card);
		cardView.UpdateText();
		
	}


	#endregion

	public void CheckPosition(){
		CardView cardView = null;

		var pos = cardView.cardNodePos.Position;

		var parent = cardView.GetParent();
		
	}

	private void CheckLeft(CardView originalCardView, Godot.Vector2 position){
		var parent = originalCardView.GetParent();
		var s = GetChildren;
	}
	
	#region Discard Methods
	public IEnumerator DiscardCard(CardView cardView)
	{
		//if DiscardAction is played twice on same card it will throw an error this code stops it

	
		if(cardView == null)
			yield break;

		hand.Remove(cardView.card);

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
		
		var layout = displayObjectsView.LayoutObjects(playerHand2D,99);

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

		/*
		var cardView = GetCardView(healAction.attachedAbility.card);

		var instance = healAnimationConstruct.Instantiate();
		cardView.AddChild(instance);

		SceneSwitcher.AudioManager.Call("create_audio","CARD_HEAL");
		foreach(var target in healAction.targets){
			var view = GetCardView((Card)target);
			if(view != null)
			view.UpdateText();
		}
		
		 */

		}

	

	void OnValidateDamage (object sender, object args) {
		var action = sender as DamageAction;
		action.perform.viewer = OnPerformDamageView;
	}

	void OnValidateCreateCard (object sender, object args) {
		var action = sender as CreateCardsAction;
		action.perform.viewer = OnPerformCreateView;
	}
	IEnumerator OnPerformCreateView (IContainer game, GameAction action) {
		var transformAction = action as CreateCardsAction;
		var attackType = transformAction.attachedAbility.GetAspect<ITargetSelector>();
		
		

		var attackCard = transformAction.attachedAbility.card;
		

		if (attackCard == null)
			yield break;
		
		
		var attackerView = GetCardView(attackCard);
		if(attackerView == null)
			yield break;
			
		

		

		attackerView.button.Call("_on_drop_card",false);
	
	

		attackerView.button.Call("_on_drop_card",true);
	}
    List<CardView> GetAllTargetsView(List<Card> targets)
    {	
		List<CardView> targetViews = new();
		foreach(Card target in targets){
			var targetView = GetCardView(target);
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
		

		var attackerView = GetCardView(attackCard);
		if(attackerView == null){
			yield return true;

			foreach(var target in attackAction.targets){
			var view = GetCardView((Card)target);
			if(view != null)
			view.UpdateText();
		}
		yield break;
		}
			

		
		attackerView.button.Call("_on_drop_card",false);
	
		if(attackAction.targets.Count <= 0){
		
			yield return true;
			
		}else{
		var targetView = GetCardView((Card)attackAction.targets[0]);
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
			var targetV = GetCardView((Card)target);
			if(targetV != null){
			targetV.button.Call("callShine");
			instance = DataManager.node.animationShake.Instantiate();
			targetV.button.AddChild(instance);
			}
		}
		
		instance = DataManager.node.animationShake.Instantiate();
		AnimationShake cameraShake= (AnimationShake)instance;
		camera2D.AddChild(cameraShake);
		
		yield return true;

		
		foreach(var target in attackAction.targets){
			var view = GetCardView((Card)target);
			if(view != null)
			view.UpdateText();
		}
		
		

		}

		attackerView.button.Call("_on_drop_card",true);
	}

	#region AddCard
	public IEnumerator AddCard (CardView cardView, bool showPreview, Ability attachedAbility, int i) {
		
		//	if(attachedAbility != null)
			if(i == 0){
				
			hand.Add(cardView.card,cardView);
			
			cardView.UpdateText();

			var layout = displayObjectsView.LayoutObjects(playerHand2D,99);
			while (layout.MoveNext ())
				yield return null;

			}else{

			hand.Add(cardView.card,cardView);
			
			cardView.UpdateText();

			var layout = displayObjectsView.LayoutObjects(enemyHand2D,99);
			while (layout.MoveNext ())
				yield return null;

			}
			
	}

	#endregion
	


	

	

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
		
		var layout = displayObjectsView.LayoutObjects(playerHand2D,99);
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
			
				action.perform.viewer = DeathCard;
		}
	}

	IEnumerator DeathCard(IContainer game, GameAction action)
	{
		
		var playAction = action as DeathAction;
		CardView cardView = GetCardView(playAction.card);
		

		if (cardView == null)
			yield break;

		hand.Remove(cardView.card);

		displayObjectsView.LayoutObjects(playerHand2D, 99);
		var discard = DeathAnimation(cardView);
		while (discard.MoveNext())
			yield return null;


	}


    #endregion
}