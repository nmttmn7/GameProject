using System.Collections;
using System.Collections.Generic;
using Godot;

using TheLiquidFire.AspectContainer;
using TheLiquidFire.Notifications;

public class ManaSystem : Aspect, IObserve {
	public const string ValueChangedNotification = "ManaSystem.ValueChangedNotification";

	public void Awake () {
		this.AddObserver(OnPerformDamageAction, Global.PerformNotification<DamageAction>(), container);
		this.AddObserver (OnPerformChangeTurn, Global.PerformNotification<ChangeTurnAction> (), container);
		this.AddObserver (OnPerformPlayCard, Global.PerformNotification<PlayCardAction> (), container);
		this.AddObserver (OnValidatePlayCard, Global.ValidateNotification<PlayCardAction> ());
	}

	public void Destroy () {
		this.RemoveObserver(OnPerformDamageAction, Global.PerformNotification<DamageAction>(), container);
		this.RemoveObserver (OnPerformChangeTurn, Global.PerformNotification<ChangeTurnAction> (), container);
		this.RemoveObserver (OnPerformPlayCard, Global.PerformNotification<PlayCardAction> (), container);
		this.RemoveObserver (OnValidatePlayCard, Global.ValidateNotification<PlayCardAction> ());
	}

	void OnPerformDamageAction(object sender, object args)
	{	
		var action = args as DamageAction;
		var mana = container.GetMatch ().CurrentPlayer.mana;
		
		
		if(container.GetMatch().CurrentPlayer.index == 1 && action.targets.Count <= 0){
		

			var manaOppose = container.GetMatch ().OpponentPlayer.mana;
			manaOppose.hitpenalty += 1;

		}
	

	}
	void OnPerformChangeTurn (object sender, object args) {
		var mana = container.GetMatch ().OpponentPlayer.mana;
		if (mana.permanent < Mana.MaxSlots)
			mana.permanent = Mana.MaxSlots;
		mana.spent = 0;
		mana.overloaded = mana.pendingOverloaded;
		mana.pendingOverloaded = 0;
		mana.temporary = 0;

	
		var opposerMana = container.GetMatch ().CurrentPlayer.mana;
		opposerMana.hitpenalty = 0;
		this.PostNotification (ValueChangedNotification, mana);
	}

	void OnPerformPlayCard (object sender, object args) {
		var action = args as PlayCardAction;
		var mana = container.GetMatch ().CurrentPlayer.mana;

		int cost = action.card.cost;

		//var poly = AugmentSystem.CheckPolymorph(action.card);
		
		//if (poly != null)
		//	cost = poly.cost;
		
		if(cost == -1){
			mana.spent += mana.permanent;
		}else{
		mana.spent += cost;
		}
		this.PostNotification (ValueChangedNotification, mana);
	}

	void OnValidatePlayCard (object sender, object args) {
		var playCardAction = sender as PlayCardAction;
		var validator = args as Validator;
		var player = container.GetMatch().players[playCardAction.card.ownerIndex];
		if (player.mana.Available < playCardAction.card.cost)
			validator.Invalidate ();
	}
}
