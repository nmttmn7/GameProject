using Godot;
using Godot.Collections;
using System;

[GlobalClass]
public partial class PackData : Resource
{	
	[ExportGroup("Variables")]
	[Export]public Array<CardData> cardList = new Array<CardData>();
	[Export]public Array<int> drawList = new Array<int>();
	

}
