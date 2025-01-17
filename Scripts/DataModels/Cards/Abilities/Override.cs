using System.Collections;
using System.Collections.Generic;

using TheLiquidFire.AspectContainer;

public class Override : Aspect {
	
	public Dictionary<string, object> Properties = new();

	public Status manaStatus;

	public string GetValue(string str){
		if(Properties.TryGetValue(str, out object value)){
			return value.ToString();
		}
			return "";
	}

	public string Save(){
		string text = "";
		
		text += "\n\"overridehealth\": {";
	//	text += "\n\"status\": " + "\"" + status + "\","; 
		text += "\n}";
	
		return text;
	}

}
