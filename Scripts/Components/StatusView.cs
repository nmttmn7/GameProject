using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TheLiquidFire.AspectContainer;
using TheLiquidFire.Notifications;

public partial class StatusView : Node
{
	[Export] PackedScene statusIconConstruct;



	[Export] HandView handView;
	public override void _Ready()
	{
		
		this.AddObserver (OnStatusValueChangedNotification, AugmentSystem.UpdateStatusNotification);
		this.AddObserver (OnValidateStatusAbility, Global.ValidateNotification<StatusAbilityAction> ());
	
	}

    public override void _ExitTree()
    {
		this.RemoveObserver (OnStatusValueChangedNotification, AugmentSystem.UpdateStatusNotification);
		this.RemoveObserver (OnValidateStatusAbility, Global.ValidateNotification<StatusAbilityAction> ());
	
    }

	

	public IEnumerator LayoutObjects(CardView cardView,Dictionary<string, Node> list, int elementIndent, bool animated = true)
	{	
		//var children = node.GetChildren();

		if(list.Count <= 0 || cardView == null)
			yield break;
		
		if(list.Count < elementIndent)
			elementIndent = list.Count;

		var overlapY = 65f;
		var overlapX = 70f;
		var width = elementIndent * overlapX;
		var xPos = -(width / 2f);
		var yPos = 0f;

		xPos += overlapX / 2;
		
		var duration = animated ? 0.25f : 0;
		Tween tween = cardView.CreateTween();
		for (int i = 0; i < list.Count; ++i)
		{


		if(i % elementIndent == 0 && i > 0){
		 yPos += overlapY;
		 xPos = -(width / 2f);
		 xPos += overlapX / 2;
		}
		
			var position = new Vector2(xPos,yPos);
			
			tween.TweenProperty(list.ElementAt(i).Value, "position", position, .05f).SetEase(Tween.EaseType.Out);
			
			xPos += overlapX;


		}

		
		
		while (tween != null && tween.IsRunning())
			yield return null;
	

	}


	void OnStatusValueChangedNotification (object sender, object args) {
		
		var notification = args as StatusNotification;
		var status = notification.status;
		var card = notification.card;
		var cardView = handView.GetView(card);

		if(cardView == null)
			return;
			
		if(cardView.statusNodes.TryGetValue(status.id, out var statusNode)) {
			UpdateStatusAnimation(status,cardView).MoveNext();
		}else{
			CreateStatusIcon(status,1f,cardView);
			
		}
		

	}
		Node CreateStatusIcon(Status status, float animationTime,CardView cardView)
	{

		var instance = statusIconConstruct.Instantiate();
		cardView.statusNode2D.AddChild(instance);
		cardView.statusNodes.Add(status.id, instance);
		StatusUI ui = instance.GetChild<StatusUI>(0);
		
		ui.statusIcon.Texture = (Texture2D)GD.Load(status.sprite);

		instance.Name = status.id;
		
		if(status.statusType == StatusTypes.INNATE)
			ui.label.Visible = false;
		else
			ui.label.Visible = true;

		
		var decr = status.decrease.ToString().ToLower();

		
		
		if(decr == "reset")
			ui.decreaseIcon.Visible = false;
		else if(System.Convert.ToInt32(decr) > 0)
			ui.decreaseIcon.Visible = false;
		else
			ui.decreaseIcon.Visible = true;

		ui.label.Text = status.value.ToString();
		Node2D node = (Node2D)instance;
		node.Scale = Vector2.Zero;

		LayoutObjects(cardView,cardView.statusNodes,4,false).MoveNext();
		var animation = StatusIconCreateAnimation((Node2D)instance, animationTime);
		animation.MoveNext();

		if(status.polyCard != null){
			cardView.UpdateText();
		}

		return instance;
	}


	public IEnumerator StatusIconCreateAnimation(Node2D instance, float animation)
    {
		Tween tween = instance.CreateTween();
			tween.TweenProperty(instance, "scale", new Vector2(1.4f, 1.4f), .2f);
			tween.TweenProperty(instance, "scale", new Vector2(1, 1), .2f);

		while (tween != null && tween.IsRunning())
		yield return null;
	}

	public void GenerateAllStatuses(CardView cardView){
		var aug = cardView.card.GetAspect<Augment>();

		if(aug == null)
		  return;

		foreach (var pair in aug.statusPairs){
			CreateStatusIcon(pair.Value.GetAspect<Status>(),0f,cardView);
		}
	}
	
	void OnValidateStatusAbility (object sender, object args) {
		var action = sender as StatusAbilityAction;
		action.perform.viewer = OnPerformStatusAbility;
	}

	IEnumerator OnPerformStatusAbility (IContainer game, GameAction action) {
		var statusAbilityAction = action as StatusAbilityAction;
		
		var status = statusAbilityAction.abilityRoot.GetAspect<Status>();
		var card = statusAbilityAction.abilityRoot.card;
		var cardView = handView.GetView(card);
		
		if(cardView == null)
			yield break;

		var nodeObj = cardView.statusNodes[status.id];

		

		var growAnimation = StatusGrowAnimation(nodeObj, status, cardView);
		while (growAnimation.MoveNext())
				yield return false;
		
		yield return true;
		

		
		if(status.value <=0){
		var statusRemoveAnimation = StatusRemoveAnimation(nodeObj, status, cardView);
		while (statusRemoveAnimation.MoveNext())
				yield return false;

		var layoutObjects = LayoutObjects(cardView,cardView.statusNodes,4,false);
		while (layoutObjects.MoveNext())
				yield return false;

		}else{
			
		var shrinkAnimation = StatusShrinkAnimation(nodeObj, status, cardView);
		shrinkAnimation.MoveNext();
				
		}
	}

	public IEnumerator StatusGrowAnimation(Node nodeObj, Status status, CardView cardView){
				

		Tween tween = CreateTween();
			tween.TweenProperty(nodeObj, "scale", new Vector2(1.4f, 1.4f), .6f);
			tween.TweenInterval(.2f);
			while (tween != null && tween.IsRunning())
				yield return null;
	}
	public IEnumerator StatusRemoveAnimation(Node nodeObj,Status status, CardView cardView){
		var statusValue = status.value.ToString();
		
		StatusUI ui = nodeObj.GetChild<StatusUI>(0);

		ui.label.Text = statusValue;	

		Tween tween = nodeObj.CreateTween();

		
			
			tween.TweenProperty(nodeObj, "scale", new Vector2(.4f, .4f), .5f).SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Elastic);
			
		
			
		
	

		while (tween != null && tween.GetTotalElapsedTime() < .4f)
				yield return false;
		
		
		RemoveStatus(nodeObj,cardView,status);
	}
	public IEnumerator StatusShrinkAnimation(Node nodeObj,Status status, CardView cardView){
	
		var statusValue = status.value.ToString();
		
	

		Tween tween = nodeObj.CreateTween();

		
		tween.TweenProperty(nodeObj, "scale", new Vector2(1f, 1f), .4f).SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Back);
		tween.TweenCallback(Callable.From(() => UpdateText(nodeObj,statusValue)));
		

		while (tween != null && tween.IsRunning())
				yield return false;
		
		
	}

	public void UpdateText(Node nodeObj, string statusValue)
    {
		StatusUI ui = nodeObj.GetChild<StatusUI>(0);
		ui.label.Text = statusValue;	
	}

	public void RemoveStatus(Node nodeObj, CardView cardView, Status status)
    {
		nodeObj.QueueFree();
		cardView.statusNodes.Remove(status.id);
		cardView.card.GetAspect<Augment>().statusPairs.Remove(status.id);
	}
	
	
		public IEnumerator UpdateStatusAnimation(Status status, CardView cardView)
    {	

		var nodeObj = cardView.statusNodes[status.id];
		var statusValue = status.value.ToString();
		
		StatusUI ui = nodeObj.GetChild<StatusUI>(0);
		ui.label.Text = statusValue;	

		Tween tween = nodeObj.CreateTween();
			tween.TweenProperty(nodeObj, "scale", new Vector2(1.4f, 1.4f), .2f);
			tween.TweenProperty(nodeObj, "scale", new Vector2(1, 1), .2f);

			while (tween != null && tween.IsRunning())
				yield return null;
			
			

			
		}
		
	
	
	

	
}
