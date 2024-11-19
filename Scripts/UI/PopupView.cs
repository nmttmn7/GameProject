using Godot;
using System;
using System.Collections.Generic;

public partial class PopupView : Node
{
	[Export] PackedScene popupConstruct; 
	List<string> popupNames = new List<string>();
	public void PopupCheck(CardView cardView){
		CreateAbilityInfo(cardView);
		CreateStatusInfo(cardView);
	}
	
	public void UpdatePopup(CardView cardView){
		DeleteChildren();
		PopupCheck(cardView);
	}

	private void CreateAbilityInfo(CardView cardView){
		Card card = cardView.card;
		var abilityRoot = card.GetAspect<AbilityRoot>();
		if(abilityRoot.abilityChain.Count > 0){
		var status = abilityRoot.abilityChain[cardView.descriptionIndex].GetAspect<Status>();

		if(status != null)
			CreatePopup(status);

		}
		
	}
	private void CreateStatusInfo(CardView cardView){
		Card card = cardView.card;

		var aug = card.GetAspect<Augment>();

		if(aug == null)
			return;

		foreach (var entry in aug.statusPairs){

			
			var status = entry.Value.GetAspect<Status>();
			CreatePopup(status);
			
		}
	}

	private void CreatePopup(Status status){

			if(popupNames.Contains(status.id))
				return;
				
			var instance = popupConstruct.Instantiate();
			var construct = (PopupConstruct)instance;
			construct.headerText.Text = status.name;
			construct.headerText.Text += " " + "[img=35]" + status.sprite + "[/img]";
			string description = status.evokeType + ": " + status.description;
			construct.descriptionText.Text = description;
			popupNames.Add(status.id);
			AddChild(instance);
	}

	public void DeleteChildren(){
		popupNames.Clear();
		var children = GetChildren();
		foreach (var child in children)
			child.QueueFree();
	}
}
