using System.Collections;
using System.Collections.Generic;
using Godot;
using TheLiquidFire.Notifications;

public partial class Clickable : Node {


	public const string ClickedNotification = "Clickable.ClickedNotification";

	public void OnRelease(){
	
		this.PostNotification (ClickedNotification, "OnRelease");

	}

	public void OnEnter(){
	
		this.PostNotification (ClickedNotification, "OnEnter");

	}

	public void OnClick(){
	
		this.PostNotification (ClickedNotification, "OnClick");

	}
	
}