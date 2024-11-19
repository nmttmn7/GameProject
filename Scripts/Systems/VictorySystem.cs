using System;
using System.Collections;
using System.Collections.Generic;
using Godot;

using TheLiquidFire.AspectContainer;

public class VictorySystem : Aspect {
	public bool IsGameOver () {
		var match = container.GetMatch ();
		foreach (Player p in match.players) {
			int c = p[Zones.Discard].Count + p[Zones.Hand].Count + p[Zones.Deck].Count;
			if (c <= 0) {
				return true;
			}
		}
		return false;
	}

	public int OO(){
		var match = container.GetMatch ();
		foreach (Player p in match.players) {
			int c = p[Zones.Discard].Count + p[Zones.Hand].Count + p[Zones.Deck].Count;
			
			if(c<= 0){

				if(p.index == 0){
					//Player empty
					return 1;
				}else{
					//Opponet empty
					return 2;

				}


			}


		}
		return 0;

	}

    internal int CompletedAllActions()
    {
        throw new NotImplementedException();
    }

}
