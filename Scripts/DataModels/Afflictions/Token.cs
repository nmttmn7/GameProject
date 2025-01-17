using Godot;
using System;
using System.Collections.Generic;

public class Token : Status
{
	private string tokenBuilder = "";
	 public override void Load(Dictionary<string, object> data, Card target, Ability castedAbility = null, StatusSystem statusSystem = null) {

		base.Load(data, target, castedAbility ,statusSystem);

		if(data.ContainsKey("nab"))
			BuildToken((string)data["nab"]);
		
	 }

	public void BuildToken(string str){
		if(tokenBuilder.Length == 0)	tokenBuilder += str;
		else	tokenBuilder += "|" + str;
	}

	public string GetToken(){
		
		return tokenBuilder;
	}

	

}
