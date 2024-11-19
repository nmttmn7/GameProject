using System.Collections;
using System.Collections.Generic;

using TheLiquidFire.Notifications;
using TheLiquidFire.AspectContainer;
using Godot;
using System.Numerics;
using System.Linq;
using TheLiquidFire.Extensions;



public partial class CardConstructorView : Node, IAspect {

    // Called when the node enters the scene tree for the first time.

	public IContainer container { 
		get {
			if (_container == null) {
				_container = new TheLiquidFire.AspectContainer.Container();
				_container.AddAspect (this);
				_container.AddAspect<ActionSystem> ();
			}
			return _container;
		}
		set {
			_container = value;
		}
	}
	IContainer _container;

	ActionSystem actionSystem;


	[Export] PackedScene cardConstruct;
	[Export] Node2D rewardNode2D;
	[Export] Node2D deckNode2D;
	[Export] StatusView statusView;
	[Export] DisplayObjectsView displayObjectsView;
	

	private Player p;
    	public override void _EnterTree()
    {	
		container.Awake ();
		actionSystem = container.GetAspect<ActionSystem> ();
	
        }

		public override void _ExitTree()
		{	
		
		}

		public override void _Process(double delta)
			{
		actionSystem.Update ();
			}

	
	
	public override void _Ready()
	{

		SaveFactory.SaveCurrentScene("res://Scenes/CardConstructorScene.tscn");




		
		SetUpPlayerCards();


	}
	public void _on_button_down(){
		SaveFactory.SaveDeck("res://UserData/Cards/Player/Deck.txt",p);
		SceneSwitcher.node.SwitchScene("res://Scenes/MapScene.tscn");
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

				CardView cardView = instance.GetChild<CardView>(0);
				cardView.card = player[Zones.Deck][i];
				cardView.UpdateText();

				statusView.GenerateAllStatuses(cardView);

		}
		displayObjectsView.LayoutObjects(deckNode2D, 99).MoveNext();

	}

	public void CombineCards(Card cardBase, CardView cardDelete){

	
		var abilityDelete = cardDelete.card.GetAspect<AbilityRoot>().abilityChain[cardDelete.GetDescriptionIndex()];
		var abilitychainBase = cardBase.GetAspect<AbilityRoot>().abilityChain;

		abilityDelete.container = cardBase;
		abilityDelete.card = cardBase;
		abilitychainBase.Add(abilityDelete);

		DiscardCard(cardDelete);

	}

	private void DiscardCard(CardView cardDelete){
		p[cardDelete.card.zone].Remove (cardDelete.card);
		cardDelete.button.Call("callFree");

	}
	
	
	
}
