using System.Collections;
using System.Collections.Generic;
using Godot;

public class Mana {
	public const int MaxSlots = 3;

	public int spent;
	public int hitpenalty;
	public int permanent;
	public int overloaded;
	public int pendingOverloaded;
	public int temporary;

	public float Unlocked {
		get {
			return Mathf.Min (permanent + temporary, MaxSlots);
		}
	}

	public int Available {
		get {
			return Mathf.Min (permanent + temporary - spent - hitpenalty, MaxSlots) - overloaded;
		}
	}
}