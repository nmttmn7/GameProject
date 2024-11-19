using Godot;
using System;
using System.Collections;
using System.Collections.Generic;

public partial class Unit :  Card, IDestructable
{
	// IDestructable
	public int hitPoints { get; set; }
	public int bulwarkPoints { get; set; }
	public int maxHitPoints { get; set; }
	

	
	public int money;
	public string Items;
	//Like slay the spire items on companion cards
	//public Dictionary<Companion, int> friends = new Dictionary<Companion, int>();
	//cards play on one turn can gain friendship points with one another
	// If friendship is on, gain more stats or play when one card is played the other is played
	
	public override void Load(Dictionary<string, object> data)
	{
		base.Load(data);
		//attack = System.Convert.ToInt32(data["attack"]);
		
		hitPoints = maxHitPoints = System.Convert.ToInt32(data["hit points"]);
		
		if(data.ContainsKey("max hit points"))
		maxHitPoints = System.Convert.ToInt32(data["max hit points"]);
		
		if(data.ContainsKey("money")){
		money = System.Convert.ToInt32(data["money"]);
		}else{
			money = 0;
		}
		
		}

    public override string Save()
    {
        string text = base.Save();
	//	text += "\n\"max hit points\": " + "\"" + maxHitPoints + "\",";
		text += "\n\"hit points\": " + "\"" + maxHitPoints + "\","; 
		text += "\n\"money\": " + "\"" + money + "\","; 
		return text;

    }
}


