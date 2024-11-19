using System.Collections;
using System.Collections.Generic;


public class PlayCardAction : GameAction {
	public Card card;

	public PlayCardAction(Card card) {
		this.card = card;
	}
}
