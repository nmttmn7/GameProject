using Godot;
using System;
using System.Collections.Generic;

public class Health : Status
{
	public int maxHealth { get; set; }
	 public override void Load(Dictionary<string, object> data, Card target, Ability castedAbility = null, StatusSystem statusSystem = null) {

		base.Load(data,target,castedAbility ,statusSystem);

		if(data.ContainsKey("maxhealth"))
			maxHealth = (int)data["maxhealth"];
		else
			maxHealth = value;
		
	 }

	public void ChangeMaxHealth(string info, string modifier, Card target, Ability castedAbility, StatusSystem statusSystem){
		

			int amount = statusSystem.ParseAbilityInfo(info.ToString(), target, castedAbility);

			

			if(modifier.Contains("+")){
				    maxHealth += amount;
					return;
	}else if(modifier.Contains("-")){
				    maxHealth -= amount;
					return;
	}else if(modifier.Contains("*"))
					maxHealth *= amount;



	}

	public override void ChangeValue(string data, string modifier, Card target, Ability castedAbility, StatusSystem statusSystem){

			string info =  data.ToString();
			info = CheckToken(info,castedAbility);

			int amount = statusSystem.ParseAbilityInfo(info, target, castedAbility);

			

			if(modifier.Contains("+")){
				    value = Mathf.Clamp(value + amount, 0, maxHealth);
					return;
	}else if(modifier.Contains("-")){
				    value = Mathf.Clamp(value - amount, 0, maxHealth);
					return;
	}else if(modifier.Contains("*"))
					value = Mathf.Clamp(value * amount, 0, maxHealth);



	}
	
}
