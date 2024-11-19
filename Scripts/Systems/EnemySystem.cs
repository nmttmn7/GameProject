using System.Collections;
using System.Collections.Generic;

using TheLiquidFire.AspectContainer;
using TheLiquidFire.Extensions;

public class EnemySystem : Aspect {

	public void TakeTurn () {
		if (PlayACard ())
			return;
		container.GetAspect<MatchSystem> ().ChangeTurn ();
	}

	bool PlayACard () {
		var system = container.GetAspect<CardSystem> ();
		var playerSystem = container.GetAspect<PlayerSystem> ();
		
		if (system.playable.Count == 0)
			return false;

		Card card;
		if(PlayerSystem.round % 2 == 0)
		card = system.playable[0];
		else
		card = system.playable[system.playable.Count - 1];

		var action = new PlayCardAction (card);
		container.Perform (action);
		return true;
	}

/*	bool Attack () {
		var system = container.GetAspect<AttackSystem> ();
		if (system.validAttackers.Count == 0 || system.validTargets.Count == 0)
			return false;
		var attacker = system.validAttackers.Random ();
		var target = system.validTargets.Random ();
		var action = new AttackAction (attacker, target);
		container.Perform (action);
		return true;
	} */
}
