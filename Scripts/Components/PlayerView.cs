using System.Collections;
using System.Collections.Generic;
using Godot;


public partial class PlayerView : Node {

	[Export]public DeckView deck;
	[Export]public HandView hand;

	public Player player { get; private set; }

	public void SetPlayer (Player player) {
		this.player = player;
	}

	public Node GetMatch (Card card) {
	
			GD.Print("No Implementation for zone");
			return null;
		
	}
}