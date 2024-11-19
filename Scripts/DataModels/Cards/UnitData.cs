using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class UnitData : CardData, IDestructable
{	
	[ExportGroup("Variables")]
    [Export] public int hitPoints { get; set; }
    [Export] public int maxHitPoints { get; set; }

	public int money;

	[ExportGroup("AbilityRoot")]
	[Export]public Array<AbilityData> abilityChain = new Array<AbilityData>();

}
