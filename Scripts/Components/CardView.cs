using Godot;
using System;
using System.Collections.Generic;
using TheLiquidFire.Notifications;


public partial class CardView : Node
{

	public Card card;
	[Export]public Node2D cardNodePos;
	[Export]public Node2D statusNode2D;
	

	[Export]public TextureRect textureRect;
	[Export]public RichTextLabel healthText;
	[Export]public RichTextLabel manaText;
	[Export]public RichTextLabel nameText;
	[Export]public RichTextLabel abilityCountText;
	[Export]public RichTextLabel moneyText;
	[Export]public RichTextLabel descriptionText;
	public int descriptionIndex = 0;
	[Export]public Button button;
	public Dictionary<string, Node> statusNodes = new();
	public void UpdateText(){
		var unit = card as Unit;
	
		if (unit != null) {
			OO();
			

			healthText.Text = "[center]" + unit.hitPoints.ToString () + "[/center]";
			
			
			
				
			nameText.Text = "[center]"  + unit.name + "[/center]" ;
			textureRect.Texture =  (Texture2D)GD.Load(unit.spritePath);
			manaText.Text = "[center]" + unit.cost.ToString () + "[/center]";

			

			if(unit.description.Count <= 1)
			abilityCountText.Text = "[center]"  + "-" + "[/center]";
			else{
			
			var i = descriptionIndex + 1;
			abilityCountText.Text = "[center]"  + i + "[/center]";

			}
			

			moneyText.Text = "[center]"  + unit.money + "[/center]";
			

			if(unit.description.Count > 0)
			descriptionText.Text = "[center]"  + unit.description[descriptionIndex] + "[/center]";
			
		}

		
	}

	public void OO(){
		var oh = card.GetAspect<OverrideHealth>();

		if(oh != null){
			healthText.Visible = false;
			var statusData = DeckFactory.Statuses[oh.status];
			var sprite = (string)statusData["sprite"];
			TextureRect textureRect = healthText.GetParent<TextureRect>();
			textureRect.Texture = (Texture2D)GD.Load(sprite);
		}
	}

	public void DescriptionTextNext(){
		var unit = card as Unit;

		if(unit.description.Count <= 0)
			return;
			
		descriptionIndex++;
		if(descriptionIndex >= unit.description.Count)
			descriptionIndex = 0;

		descriptionText.Text = "[center]"  + unit.description[descriptionIndex] + "[/center]";

		if(unit.description.Count <= 1)
			abilityCountText.Text = "[center]"  + "-" + "[/center]";
			else{
			
			var i = descriptionIndex + 1;
			abilityCountText.Text = "[center]"  + i + "[/center]";

			}
	}
	

	public void DisplayNewText(string str){
		descriptionText.Text = str;
	}

	public int GetDescriptionIndex(){
		return descriptionIndex;
	}
}
