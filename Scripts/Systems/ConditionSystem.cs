using System.Collections;
using System.Collections.Generic;

using TheLiquidFire.AspectContainer;
using TheLiquidFire.Notifications;
using System;

public class ConditionSystem : Aspect, IObserve {
	public void Awake () {
		this.AddObserver (OnPerformCheckCondition, Global.PerformNotification<AbilityAction> (), container);
	}

	public void Destroy () {
		this.RemoveObserver (OnPerformCheckCondition, Global.PerformNotification<AbilityAction> (), container);
	}

	void OnPerformCheckCondition (object sender, object args) {
		var action = args as AbilityAction;
		
		
	}
}
