
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

		this.AddObserver (OnPerformGrantMaxHealthAction, Global.PerformNotification<GrantMaxHealthAction> (), container);
	//	this.RemoveObserver (OnFilterAttackTargets, AttackSystem.FilterTargetsNotification, container);
	}

	void OnPerformHealAction (object sender, object args) {
		var action = args as HealAction;
		var augmentSystem = container.GetAspect<AugmentSystem> ();
		var status = action.attachedAbility.abilityRoot.GetAspect<Status>();
		
		string str =  action.attachedAbility.userInfo.ToString().ToLower();
		
		if(str.Contains("skip"))
			return;

		foreach (IDestructable target in action.targets) {
			
			
			int amount = augmentSystem.ParseAbilityInfo(str, (Card)target, action.attachedAbility);
			target.hitPoints = Mathf.Clamp(target.hitPoints + amount, 0, target.maxHitPoints);
		
		}

	}

	void OnPerformDamageAction (object sender, object args) {
	
		var action = args as DamageAction;
		var augmentSystem = container.GetAspect<AugmentSystem> ();
		var status = action.attachedAbility.abilityRoot.GetAspect<Status>();
		

		string str =  action.attachedAbility.userInfo.ToString().ToLower();

		if(str.Contains("skip"))
			return;

	
		foreach (IDestructable target in action.targets) {
			
			
			int amount = augmentSystem.ParseAbilityInfo(str, (Card)target, action.attachedAbility);
			target.hitPoints = Mathf.Clamp(target.hitPoints - amount, 0, target.maxHitPoints);

			
		
		}
	}

	void OnPerformGrantMaxHealthAction (object sender, object args) {
		var action = args as GrantMaxHealthAction;
		var augmentSystem = container.GetAspect<AugmentSystem> ();
		var status = action.attachedAbility.abilityRoot.GetAspect<Status>();
		

		string str =  action.attachedAbility.userInfo.ToString().ToLower();
		
		if(str.Contains("skip"))
			return;


		foreach (IDestructable target in action.targets) {
			int amount = augmentSystem.ParseAbilityInfo(str, (Card)target, action.attachedAbility);
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