using Godot;
using System;
using System.Collections.Generic;
using TheLiquidFire.Notifications;


public partial class CardView : Node
{

	public Card card;
	
	[Export]public Node2D cardNodePos;
	[Export]public Node2D statusNode2D;
	

	
	[ExportGroup("Health")]
	[Export]public TextureRect healthTexture;
	[Export]public RichTextLabel healthText;
	[ExportGroup("Mana")]
	[Export]public TextureRect manaTexture;
	[Export]public RichTextLabel manaText;
	[ExportGroup("Ability")]
	[Export]public TextureRect abilityCountTexture;
	[Export]public RichTextLabel abilityCountText;

	[ExportGroup("AbilityReader")]
	[Export]public RichTextLabel abilityReaderText;

	[ExportGroup("Cardview")]
	[Export]public TextureRect textureRect;
	[Export]public RichTextLabel nameText;
	
	[Export]public RichTextLabel moneyText;
	[Export]public RichTextLabel descriptionText;
	public int descriptionIndex = 0;
	[Export]public Button button;

	public Dictionary<string, Node> statusNodes = new();
	public void UpdateText(){
		var unit = card as Unit;
	
		if (unit != null) {
			
			UpdateHealth();
			UpdateMana();
			UpdateAbilityChain();
			
			
			
			
				
			nameText.Text = "[center]"  + unit.name + "[/center]" ;
			textureRect.Texture =  (Texture2D)GD.Load(unit.spritePath);
		

			

			
			

			moneyText.Text = "[center]"  + unit.money + "[/center]";
			

			UpdateDescription();
			
		}

		
	}
	void LoadView(){
		UpdateMana();
	}

	public void UpdateHealth(){

		var afflictions = card.GetAspect<Afflictions>();

		if(afflictions.statusPairs.TryGetValue("health", out var value)){
			
			healthTexture.Texture = (Texture2D)GD.Load(value.sprite);
			Health health = value as Health;
			if(health != null)
				healthText.Text = "[center]" + health.value.ToString() + "/" + health.maxHealth + "[/center]";
			else
				healthText.Text = "[center]" + value.value.ToString() + "[/center]";
		}

		//healthText.Text = "[center]" + unit.hitPoints.ToString () + "/" + unit.maxHitPoints.ToString () + "[/center]";



	}

	public void UpdateMana(){

		manaText.Text = "[center]" + card.cost.ToString () + "[/center]";
		
		var afflictions = card.GetAspect<Afflictions>();

		if(afflictions.statusPairs.TryGetValue("mana", out var value))
			manaTexture.Texture = (Texture2D)GD.Load(value.sprite);
		

		
	}

	public void UpdateAbilityChain(){

		
		
		var afflictions = card.GetAspect<Afflictions>();

		if(afflictions.statusPairs.TryGetValue("abilitychain", out var value)){

		abilityCountTexture.Texture = (Texture2D)GD.Load(value.sprite);
		abilityCountText.Text = "[center]" + value.value + "[/center]";

		}

	}

	public void UpdateDescription(){

		AbilityRoot abilityRoot = card.GetAspect<AbilityRoot>();

		if(abilityRoot.abilityChain.Count - 1 < descriptionIndex)
			descriptionIndex = 0;

		
			var i = descriptionIndex + 1;

			abilityReaderText.Text = "[center]"  + i + "/" + abilityRoot.abilityChain.Count + "[/center]";
		
		

			descriptionText.Text = "[center]"  + abilityRoot.abilityChain[descriptionIndex].description + "[/center]";


	}



	public void DescriptionTextNext(){
		descriptionIndex++;
		UpdateDescription();
	}
	

	public void DisplayNewText(string str){
		descriptionText.Text = str;
	}

	public int GetDescriptionIndex(){
		return descriptionIndex;
	}
}
