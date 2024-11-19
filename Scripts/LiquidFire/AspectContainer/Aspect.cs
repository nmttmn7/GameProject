using System.Collections;
using System.Collections.Generic;


namespace TheLiquidFire.AspectContainer
{
	public interface IAspect {
		IContainer container { get; set; }
	}

	public class Aspect : IAspect {
		public IContainer container { get; set; }
	}
}