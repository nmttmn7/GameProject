using Godot;
using System;
using System.Collections;
using System.Collections.Generic;

public partial class Unit :  Card
{
	// IDestructable
	
	
	public string HP;

	
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
		
		
		
		
		
		if(data.ContainsKey("money")){
		money = System.Convert.ToInt32(data["money"]);
		}else{
			money = 0;
		}

		this.AddAspect<Afflictions>();
		
		}

    public override string Save()
    {
        string text = base.Save();
	//	text += "\n\"max hit points\": " + "\"" + maxHitPoints + "\",";
	
		text += "\n\"money\": " + "\"" + money + "\","; 
		return text;

    }
}


