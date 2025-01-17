using Godot;
using System;
using System.Collections.Generic;

public partial class PopupView : Node
{
	[Export] PackedScene popupConstruct; 
	List<string> popupNames = new List<string>();
	public void PopupCheck(CardView cardView){
		CreateStatusInfo(cardView);
		CreateAbilityInfo(cardView);
		
	}
	
	public void UpdatePopup(CardView cardView){
		DeleteChildren();
		PopupCheck(cardView);
	}

	private void CreateAbilityInfo(CardView cardView){
		Card card = cardView.card;
		var abilityRoot = card.GetAspect<AbilityRoot>();
		if(abilityRoot.abilityChain.Count > 0){
		var statusData = abilityRoot.abilityChain[cardView.descriptionIndex].GetAspect<StatusData>();

		if(statusData != null)
			CreatePopup(statusData);

		}
		
	}
	private void CreateStatusInfo(CardView cardView){
		Card card = cardView.card;

		var afflictions = card.GetAspect<Afflictions>();

		if(afflictions == null)
			return;

		foreach (var entry in afflictions.statusPairs){

			if(entry.Key != "health" && entry.Key != "mana" && entry.Key != "abilitychain"){
			var status = entry.Value;
			CreatePopup(status);
			}
			
		}
	}

	private void CreatePopup(StatusData statusData){
			
			string id = (string)statusData.data["id"];
			id = id.ToLower();

			if(popupNames.Contains(id))
				return;

			var data = DeckFactory.Afflictions[id];
				
			var instance = popupConstruct.Instantiate();
			var construct = (PopupConstruct)instance;

			construct.headerText.Text = (string)data["name"];
			construct.headerText.Text += " " + "[img=35]" + (string)data["sprite"] + "[/img]";

			string description = "";

			
				description = (string)data["evokeType"] + ": " + (string)data["description"];

			construct.descriptionText.Text = description;
			popupNames.Add(id);
			AddChild(instance);
	}

	private void CreatePopup(Status status){

			if(popupNames.Contains(status.id))
				return;
				
			var instance = popupConstruct.Instantiate();
			var construct = (PopupConstruct)instance;
			construct.headerText.Text = status.name;
			construct.headerText.Text += " " + "[img=35]" + status.sprite + "[/img]";

			string description = "";

			if(status.statusType == StatusTypes.STACKABLE){

				description = status.decrease + ": " + status.description;
			}else
				description = status.evokeType + ": " + status.description;

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
