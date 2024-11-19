using Godot;
using System;

[GlobalClass]
public partial class AbilityData : Resource
{	
	[Export] public AbilityActionTypes actionName;
	[Export] public string userInfo;
	[Export] public int count;
	
	[Export] TargetSelectorData targetSelectorData;

}
