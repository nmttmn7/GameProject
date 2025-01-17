using System;
using System.Collections;
using System.Collections.Generic;
using Godot;

using TheLiquidFire.AspectContainer;
using TheLiquidFire.Notifications;

public class Afflictions : TheLiquidFire.AspectContainer.Container, IAspect {
	public IContainer container { get; set; }
	public Card card { get { return container as Card; } }

	public Dictionary<string, Status> statusPairs = new Dictionary<string, Status>();

	public Dictionary<string,Dictionary<string, Status>> p = new Dictionary<string,Dictionary<string, Status>>();
	public string Save(){
		string text = "";
		text += "\n\"afflictions\": [";
		foreach (var pair in statusPairs)
			text += pair.Value.Save();
		text += "\n]";
		return text;
	}



	public Status GetStatus(string str){
		if(statusPairs.TryGetValue(str, out Status status)) return status;

		return null;
	}

	public void ReplaceStatus(string str, Status newStatus){

		if(statusPairs.TryGetValue(str, out Status status))
			statusPairs[str] = newStatus;
		else
			statusPairs.Add(str,newStatus);
		

	}

	public int GetStatusINT(string str){
		if(statusPairs.TryGetValue(str, out Status status)) return status.value;

		return 0;
	}

}