using System.Collections;
using System.Collections.Generic;


public class DeathAction : GameAction {
	public Card card;
	
	public DeathAction (Card card,Player player) {
		this.card = card;
		this.player = player;
	}
}
