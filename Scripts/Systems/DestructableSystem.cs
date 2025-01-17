
using System.Collections.Generic;
using Godot;

using TheLiquidFire.AspectContainer;
using TheLiquidFire.Notifications;

public class DestructableSystem : Aspect, IObserve {
	public void Awake () {
		this.AddObserver (OnPerformDamageAction, Global.PerformNotification<DamageAction> (), container);
		this.AddObserver (OnPerformHealAction, Global.PerformNotification<HealAction> (), container);

		this.AddObserver (OnPerformGrantMaxHealthAction, Global.PerformNotification<GrantMaxHealthAction> (), container);
	//	this.AddObserver (OnFilterAttackTargets, AttackSystem.FilterTargetsNotification, container);
	}

	public void Destroy () {
		this.RemoveObserver (OnPerformDamageAction, Global.PerformNotification<DamageAction> (), container);
		this.RemoveObserver (OnPerformHealAction, Global.PerformNotification<HealAction> (), container);

		this.RemoveObserver (OnPerformGrantMaxHealthAction, Global.PerformNotification<GrantMaxHealthAction> (), container);
	//	this.RemoveObserver (OnFilterAttackTargets, AttackSystem.FilterTargetsNotification, container);
	}


	void OnPerformGrantMaxHealthAction (object sender, object args) {
		var action = args as GrantMaxHealthAction;
		
		string str =  action.attachedAbility.GetInfo();
		
		if(str.Contains("skip"))
			return;

		foreach (Card target in action.targets) {
			
			GrantHealth(str, target, action.attachedAbility);
			
		}
		
	}

	void OnPerformHealAction (object sender, object args) {
		var action = args as HealAction;
		
		string str =  action.attachedAbility.GetInfo();
		
		if(str.Contains("skip"))
			return;

		foreach (Card target in action.targets) {
			
			IncreaseHealth(str, target, action.attachedAbility);
			
		
		}

	}
	
	

/*	void OnPerformDamageAction (object sender, object args) {
	
		var action = args as DamageAction;

		string str =  action.attachedAbility.userInfo.ToString().ToLower();

		if(str.Contains("skip"))
			return;

		var statusSystem = container.GetAspect<StatusSystem> ();
		
		var card = action.attachedAbility.card;
		//var status = action.attachedAbility.abilityRoot.GetAspect<Stat>();
		
		
		int weak = card.GetAspect<Afflictions>().GetStatusINT("weak");

	
		foreach (IDestructable target in action.targets) {
			
			Unit unit = (Unit)target;
			
			
			int amount = statusSystem.ParseAbilityInfo(str, (Card)target, action.attachedAbility);
			amount = amount - weak;
			if(amount < 0)
				amount = 0;

			


			ReduceHealth(unit,amount);
			

			
		
		}
	}
 */
 
	void GrantHealth(string amount, Card card, Ability castedAbility) {

		Afflictions afflictions = card.GetAspect<Afflictions>();
		var statusSystem = container.GetAspect<StatusSystem> ();



		Health health = afflictions.GetStatus("health") as Health;
		if(health != null)
			health.ChangeMaxHealth(amount,"+",card,castedAbility,statusSystem);

		statusSystem.UpdateStatus(card, health);


	}


	void IncreaseHealth(string amount, Card card, Ability castedAbility) {

		Afflictions afflictions = card.GetAspect<Afflictions>();
		var statusSystem = container.GetAspect<StatusSystem> ();



		Status health = afflictions.GetStatus("health");

		if(health != null) 
			health.ChangeValue(amount,"+",card,castedAbility,statusSystem);

		statusSystem.UpdateStatus(card, health);

	}

	void OnPerformDamageAction (object sender, object args) {
		var action = args as DamageAction;
		
		string str =  action.attachedAbility.GetInfo();
		
		if(str.Contains("skip"))
			return;

		foreach (Card target in action.targets) {
			
			ReduceHealth(str, target, action.attachedAbility);
			
		
		}

	}

	void ReduceHealth(string amount, Card card, Ability castedAbility) {

		Afflictions afflictions = card.GetAspect<Afflictions>();
		var statusSystem = container.GetAspect<StatusSystem> ();



		Status health = afflictions.GetStatus("health");

		if(health != null)
			health.ChangeValue(amount,"-",card,castedAbility,statusSystem);
				
		statusSystem.UpdateStatus(card, health);

	}

	

	
	



	void OnFilterAttackTargets (object sender, object args) {
		var candidates = args as List<Card>;
		for (int i = candidates.Count - 1; i >= 0; --i) {
			var destructable = candidates [i] as Card;
			if (destructable == null)
				candidates.RemoveAt (i);
		}
	}
}