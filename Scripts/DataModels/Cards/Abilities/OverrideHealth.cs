using System.Collections;
using System.Collections.Generic;

using TheLiquidFire.AspectContainer;

public class OverrideHealth : Aspect {
	public string status;
	

	public string Save(){
		string text = "";
		
		text += "\n\"overridehealth\": {";
		text += "\n\"status\": " + "\"" + status + "\","; 
		text += "\n}";
	
		return text;
	}
}
