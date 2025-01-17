using System.Collections;
using System.Collections.Generic;

using TheLiquidFire.AspectContainer;

public class AbilityRoot : Container, IAspect {
	public IContainer container { get; set; }
	
	public List<Ability> abilityChain = new List<Ability>();

	//List<Ability> L = new List<Ability> ( new Ability[10] );
	
	public int chainMAX;


}