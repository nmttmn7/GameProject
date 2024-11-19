using System;
using System.Collections;
using System.Collections.Generic;


[Flags]
public enum Zones {
	None = 0,
	Deck = 1 << 0,
	Hand = 1 << 1,
	Discard = 1 << 2,
	Graveyard = 1 << 3,
	Secrets = 1 << 4,
	

	Active = Deck | Hand | Discard 
	
}

public static class ZonesExtensions {
	public static bool Contains (this Zones source, Zones target) {
		return (source & target) == target;
	}
}