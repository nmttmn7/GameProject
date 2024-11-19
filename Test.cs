using System.Collections;
using System.Collections.Generic;
using Godot;

using TheLiquidFire.AspectContainer;
using TheLiquidFire.Notifications;

public class Test : Aspect, IObserve
{
    public void Awake()
    {
       this.AddObserver (OnDeathReaperNotification, ActionSystem.deathReaperNotification);
	   
    }

    public void Destroy()
    {
       this.AddObserver (OnDeathReaperNotification, ActionSystem.deathReaperNotification);
    }
	void OnDeathReaperNotification (object sender, object args) {
		GD.Print("WOW");
	}
}
