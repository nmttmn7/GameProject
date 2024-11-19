using Godot;
using System;
using System.Collections.Generic;

public partial class StoreView : Node
{
	// Called when the node enters the scene tree for the first time.
	[Export] public Node2D deckNode;
	[Export] PackedScene cardConstruct;
	public override void _Ready()
	{

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void SetUpPlayerCards()
    {
		
	Player p = new Player(0);

	List<Card> deck = new();

		if(FileFactory.Contains("res://UserData/Cards/Player/Deck.txt", "deck")){
			
			deck = DeckFactory.CreateDeck("res://UserData/Cards/Player/Deck.txt", p.index);

		}else{

			deck = DeckFactory.CreateDeck ("res://UserData/Cards/Player/FirstStarterPack.txt", p.index);
		}

		p [Zones.Deck].AddRange (deck);
	
		Draw(p);
		
    }

	private void Draw(Player player){
		
		for (int i = 0; i < player[Zones.Deck].Count; ++i) {
			
			
			
			var instance = cardConstruct.Instantiate();

			
			instance.GetChild(0).GetParent<Node2D>().Visible = true;
		
		
			deckNode.AddChild(instance);
			
			
			CardView cardView = instance.GetChild<CardView>(0);


		
			cardView.card = player[Zones.Deck][i];
	
			cardView.cardNodePos.Position = new Vector2(0, 0);
			
		


		}

	}

	
}
