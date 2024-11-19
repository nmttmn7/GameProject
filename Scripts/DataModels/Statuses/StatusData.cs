using Godot;
using System;
using System.Collections.Generic;

[Tool]
[GlobalClass]
public partial class StatusData : Resource
{	
	[ExportGroup("Stat")]
	[Export] public string id = "";
	[Export] public string name = "";
	
	[Export] public Texture2D sprite;
	[Export] public bool value_ability = true;

	[ExportGroup("Types")]
	[Export] public StatusTypes statusTypes;
	[Export] public EvokeTypes evokeType;
	
	[ExportGroup("Decrement", "decrement")]
	[Export] public bool decrement_resetOverride = false;
	[Export(PropertyHint.Range, "-10,10,1")] public float decrement_amount = 1;

	[ExportGroup("Abilities")]
	[Export] public Godot.Collections.Array<AbilityData> abilityChain = new();
	
}






