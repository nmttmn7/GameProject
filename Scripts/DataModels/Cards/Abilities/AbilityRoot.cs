using System.Collections;
using System.Collections.Generic;

using TheLiquidFire.AspectContainer;

public class AbilityRoot : Container, IAspect {
	public IContainer container { get; set; }
	public Card card { get { return container as Card; } }
	
	public List<Ability> abilityChain = new List<Ability>();


}