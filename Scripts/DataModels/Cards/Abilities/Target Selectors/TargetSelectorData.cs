using Godot;
using System;

[GlobalClass]
public partial class TargetSelectorData : Resource
{	
	[Export] public TargetSelectorTypes type;
	
	[Export] MarkData markData;
} 
