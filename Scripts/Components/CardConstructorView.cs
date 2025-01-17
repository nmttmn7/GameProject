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
	[Export] AfflictionView AfflictionView;
	[Export] DisplayObjectsView displayObjectsView;
	

	private Player p;
    	public override void _EnterTree()
    {	
		container.Awake ();
		container.AddAspect<DataSystem>();
		container.AddAspect<StatusSystem>();
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
		SaveFactory.SaveDeck(DataManager.playerdeckPath,p);
		SceneSwitcher.node.SwitchScene("res://Scenes/MapScene.tscn");
	}
	private void SetUpPlayerCards()
    {
		
	p = new Player(0);

	List<Card> deck = new();

		if(FileFactory.Contains(DataManager.playerdeckPath, "deck")){
			
			deck = DeckFactory.CreateDeck(DataManager.playerdeckPath, p.index);

		}else{

			deck = DeckFactory.CreateDeck (DataManager.placeHolderDeck, p.index);
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

				AfflictionView.GenerateAllStatuses(cardView);

		}
		displayObjectsView.LayoutObjects(deckNode2D, 99).MoveNext();

	}

	public void CombineCards(Card cardBase, CardView cardDelete){

	
		bool success = DeckFactory.CombineCards(cardBase,cardDelete,false);
		if(success)
		DiscardCard(cardDelete);

	}

	private void DiscardCard(CardView cardDelete){
		p[cardDelete.card.zone].Remove (cardDelete.card);
		cardDelete.button.Call("callFree");

	}
	
	
	
}
