using System;
using System.Collections;
using System.Collections.Generic;
using Godot;

using TheLiquidFire.AspectContainer;
using TheLiquidFire.Notifications;

public class Augment : TheLiquidFire.AspectContainer.Container, IAspect {
	public IContainer container { get; set; }
	public Card card { get { return container as Card; } }

	public Dictionary<string, AbilityRoot> statusPairs = new Dictionary<string, AbilityRoot>();

	public string Save(){
		string text = "";

		text += "\n\"afflictions\": [";
		foreach(var value in statusPairs.Values){
			var stat = value.GetAspect<Status>();
			if(stat.permanent){
				text += "\n{";
				text += stat.Save();
				text += "\n}";
			}
		}
		text += "\n]";
		
	
		return text;
	}

}