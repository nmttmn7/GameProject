using System;
using System.Collections;
using System.Collections.Generic;
using Godot;

using TheLiquidFire.AspectContainer;
[GlobalClass]
public partial class CardData : Resource{
	public string id;
	public string name;
	public Rarities rarity;
	public string spritePath;
	public List<string> description = new();
	public int cost;
	public int orderOfPlay = int.MaxValue;
	public int ownerIndex;
	public Zones zone = Zones.Deck;

	
}