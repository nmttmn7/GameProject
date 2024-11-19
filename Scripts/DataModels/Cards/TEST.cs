using Godot;
using System;

[GlobalClass]
public partial class TEST : Resource
{
	[Export] public string name = "";
	[Export] public Rarities raritiy = Rarities.Common;
	
}
