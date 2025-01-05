
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

	void OnPerformHealAction (object sender, object args) {
		var action = args as HealAction;
		var statusSystem = container.GetAspect<StatusSystem> ();
		var status = action.attachedAbility.abilityRoot.GetAspect<Stat>();
		
		string str =  action.attachedAbility.userInfo.ToString().ToLower();
		
		if(str.Contains("skip"))
			return;

		foreach (IDestructable target in action.targets) {
			
			
			int amount = statusSystem.ParseAbilityInfo(str, (Card)target, action.attachedAbility);
			target.hitPoints = Mathf.Clamp(target.hitPoints + amount, 0, target.maxHitPoints);
		
		}

	}
	

	void OnPerformDamageAction (object sender, object args) {
	
		var action = args as DamageAction;
		var statusSystem = container.GetAspect<StatusSystem> ();
		
		var status = action.attachedAbility.abilityRoot.GetAspect<Stat>();
		Unit castUnit = (Unit)action.attachedAbility.card;
		int weak = StatusSystem.GetStatusValue(castUnit, "weak");
		string str =  action.attachedAbility.userInfo.ToString().ToLower();

		if(str.Contains("skip"))
			return;

	
		foreach (IDestructable target in action.targets) {
			
			Unit unit = (Unit)target;
			
			
			int amount = statusSystem.ParseAbilityInfo(str, (Card)target, action.attachedAbility);
			amount = amount - weak;
			if(amount < 0)
				amount = 0;

			amount = CheckOverrideHealth(unit, amount);
			target.hitPoints = Mathf.Clamp(target.hitPoints - amount, 0, target.maxHitPoints);
			

			
		
		}
	}

	

	int CheckOverrideHealth(Unit unit, int amount){
		var oh = unit.GetAspect<OverrideHealth>();

		if(oh == null)
			return amount;

		var statusSystem = container.GetAspect<StatusSystem> ();
		var status = StatusSystem.GetStatus(unit, oh.status);

		if(status == null)
			return amount;
			
		int statusValue = status.value;
		
		statusSystem.DecreaseStatus(status, amount, 1);
		amount = Mathf.Max(amount - statusValue, 0);
		return amount;
		
	}

	void OnPerformGrantMaxHealthAction (object sender, object args) {
		var action = args as GrantMaxHealthAction;
		var statusSystem = container.GetAspect<StatusSystem> ();
		var status = action.attachedAbility.abilityRoot.GetAspect<Stat>();
		

		string str =  action.attachedAbility.userInfo.ToString().ToLower();
		
		if(str.Contains("skip"))
			return;


		foreach (IDestructable target in action.targets) {
			int amount = statusSystem.ParseAbilityInfo(str, (Card)target, action.attachedAbility);
			target.maxHitPoints += amount;
		}
	}
	



	void OnFilterAttackTargets (object sender, object args) {
		var candidates = args as List<Card>;
		for (int i = candidates.Count - 1; i >= 0; --i) {
			var destructable = candidates [i] as IDestructable;
			if (destructable == null)
				candidates.RemoveAt (i);
		}
	}
}