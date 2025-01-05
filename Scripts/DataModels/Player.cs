using System.Collections;
using System.Collections.Generic;
using Godot;
using TheLiquidFire.Extensions;


public class Player {
	public const int maxDeck = 30;
	public const int maxHand = 10 + 1;
	//The plus one is for this scenario
	//A extra slot is needed for a card that draws more cards but if discarded after
	// 11 cards, 1 for the card that is used for [drawcardaction] then is discards to leave a total of 10 cards
	public const int maxBattlefield = 7;
	public const int maxSecrets = 5;

	public readonly int index;
	public List<int> drawList;
	public int drawI = 0;
	public ControlModes mode;
	public Mana mana = new Mana ();


	public List<Card> weapon = new List<Card> (1);
	public List<Card> deck = new List<Card> ();
	public List<Card> hand = new List<Card> (maxHand);
	public List<Card> discard = new List<Card> ();
	public Dictionary<string,List<Card>> power = new();
	public List<Card> secrets = new List<Card> (maxSecrets);
	public List<Card> graveyard = new List<Card> ();

	public List<Card> this[Zones z] {
		get {
			switch (z) {
			case Zones.Deck:
				return deck;
			case Zones.Hand:
				return hand;
			case Zones.Discard:
				return discard;
			case Zones.Secrets:
				return secrets;
			case Zones.Graveyard:
				return graveyard;
			default:
				return null;
			}
		}
	}

	public Player (int index) {
		this.index = index;
	}
	public int DrawAmount(){
		int amount = drawList[drawI];
		drawI++;
		if(drawI >= drawList.Count)
			drawI = 0;
		return amount;
	}
	public List<Card> GetCardName(Player player, Zones zone, string str){
		
		var cards = new List<Card> ();

				for(int i = 0; i < player[zone].Count; i++){
					Card card = player[zone][i];
					string name;
			//		Card poly = AugmentSystem.CheckPolymorph(card);
			//		if(poly != null)
			//			name = poly.name.ToLower();
			//		else
						name = card.name.ToLower();

						if(name.Contains(str.ToLower())){
							cards.Add(card);
						}
					
				}
			
			
		
		return cards;
	}
	
	public Card GetACard(){
		
		if(this[Zones.Deck].Count > 0)
		return this[Zones.Deck].First();
		else if(this[Zones.Discard].Count > 0)
		return this[Zones.Discard].First();
		else
		return this[Zones.Hand].First();
	}
	
}