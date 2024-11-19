using DialogueManagerRuntime;
using Godot;
using System;
using System.Collections.Generic;

public partial class EncounterView : Node
{
    [Export] Resource OO;

    [Export] PackedScene cardConstruct;
    [Export] DisplayObjectsView displayObjectsView;

    [Export] Node2D deckNode2D;
	[Export] StatusView statusView;
    private Player p;
	public override void _Ready()
	{
        DialogueManager.ShowDialogueBalloon(OO,"start");
        SetUpPlayerCards();
    }

    private void SetUpPlayerCards()
    {
		
	p = new Player(0);

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
				deckNode2D.AddChild(instance);
				instance.GetChild<CardView>(0).card = player[Zones.Deck][i];
				instance.GetChild<CardView>(0).UpdateText();
	

		}
		displayObjectsView.LayoutObjects(deckNode2D, 99).MoveNext();

	}

    
}
