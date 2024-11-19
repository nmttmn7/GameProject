using System.Collections;
using System.Collections.Generic;

using TheLiquidFire.AspectContainer;

public class Target : Aspect {
	public bool required;
	public Mark preferred;
	public Mark allowed;
	public Card selected;

	public string Save(){
		string text = "";
		text += "\n\"target\": {";
		text += "\n\"allowed\": {";
		text += "\n\"alliance\": " + "\"" + allowed.alliance + "\",";
		text += "\n\"zone\": " + "\"" + allowed.zones + "\"";
		text += "\n},";

		text += "\n\"preferred\": {";
		text += "\n\"alliance\": " + "\"" + preferred.alliance + "\",";
		text += "\n\"zone\": " + "\"" + preferred.zones + "\"";
		text += "\n}";
		text += "\n}";

		return text;
	}
}
