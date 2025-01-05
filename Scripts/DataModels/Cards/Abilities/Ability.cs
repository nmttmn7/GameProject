using System.Collections;
using System.Collections.Generic;

using TheLiquidFire.AspectContainer;

public class Ability : Container, IAspect {
	public IContainer container { get; set; }
	public AbilityRoot abilityRoot { get { return container as AbilityRoot; } }
	public Card card {get; set;}
	public string actionName { get; set; }
	public object abilityCount { get; set; }
	public object userInfo { get; set; }
	public Ability evokedAbility { get; set; }
	public int chainPosition {get; set;}
	
	public string Save(){
		string text = "";
		text += "\n{";

		text += "\n\"action\": " + "\"" + actionName + "\","; 
		text += "\n\"info\": " + "\"" + userInfo.ToString() + "\","; 
		text += "\n\"count\": " + "\"" + abilityCount.ToString() + "\","; 
		
		var ITargetSelector = this.GetAspect<ITargetSelector>();
		if(ITargetSelector != null)
		text += ITargetSelector.Save();

		var status = this.GetAspect<StatusData>();
		if(status != null){
		text += "\n\"status\": {"; 
		text += status.Save();
		text += "\n}";
		}
		var condition = this.GetAspect<Condition>();
		if(condition != null)
		text += condition.Save();

		text += "\n}";
		return text;
	}

	
}