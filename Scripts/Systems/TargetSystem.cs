using System.Collections;
using System.Collections.Generic;
using Godot;
using TheLiquidFire.AspectContainer;
using TheLiquidFire.Notifications;
using TheLiquidFire.Extensions;

public class TargetSystem : Aspect, IObserve {
	public void Awake () {
		this.AddObserver (OnValidatePlayCard, Global.ValidateNotification<PlayCardAction> ());
	}

	public void Destroy () {
		this.RemoveObserver (OnValidatePlayCard, Global.ValidateNotification<PlayCardAction> ());
	}

	public void AutoTarget (Card card, ControlModes mode, bool OO) {
		if(OO){

		var target = card.GetAspect<Target> ();
		if (target == null)
			return;
		var mark = mode == ControlModes.Computer ? target.preferred : target.allowed;
		var candidates = GetMarks (card, mark);
		target.selected = candidates.Count > 0 ? candidates.Random() : null;
	
		}else if (!OO){

		var playerSystem = container.GetAspect<PlayerSystem> ();


		var target = card.GetAspect<Target> ();
		if (target == null)
			return;
		var mark = mode == ControlModes.Computer ? target.preferred : target.allowed;
		var candidates = GetMarks (card, mark);
		
		if(PlayerSystem.round % 2 == 0)
		target.selected = candidates.Count > 0 ? candidates.First() : null;
		else
		target.selected = candidates.Count > 0 ? candidates.Last() : null;

		}
		
	}

	public List<Card> GetMarks (Card source, Card selected, Condition condition = null) {
		var marks = new List<Card> ();
		Mark mark = new();

		if(selected == null)
		return marks;

		if(selected.ownerIndex == 0){
		mark.alliance = Alliance.Ally;
		mark.zones = selected.zone;
		}else{
		mark.alliance = Alliance.Enemy;
		mark.zones = selected.zone;
		}
		
		var players = GetPlayers (source, mark);
		foreach (Player player in players) {
			var cards = GetCards (source, mark, player, condition);
			marks.AddRange (cards);
		}
		return marks;
	}
	public List<Card> GetMarks (Card source, Mark mark, Condition condition = null) {
		var marks = new List<Card> ();
		
		var players = GetPlayers (source, mark);
		foreach (Player player in players) {
			var cards = GetCards (source, mark, player, condition);
			marks.AddRange (cards);
		}

		
		if(marks.Count == 0)
			marks.Add(source);

		return marks;
	}

	List<Player> GetPlayers (Card source, Mark mark) {
		var dataSystem = container.GetAspect<DataSystem> ();
		var players = new List<Player> ();
		var pair = new Dictionary<Alliance, Player> () {
			{ Alliance.Ally, dataSystem.match.players[source.ownerIndex] }, 
			{ Alliance.Enemy, dataSystem.match.players[1 - source.ownerIndex] }
		};
		foreach (Alliance key in pair.Keys) {
			if (mark.alliance.Contains (key)) {
				players.Add (pair[key]);
			}
		}
		return players;
	}

	List<Card> GetCards (Card source, Mark mark, Player player, Condition condition = null) {
		var cards = new List<Card> ();
		var zones = new Zones[] { 
			Zones.Deck, 
			Zones.Hand,
			Zones.Discard, 
			Zones.Secrets, 
			Zones.Graveyard 
		};
		foreach (Zones zone in zones) {
			if (mark.zones.Contains (zone)) {
				
				for(int i = 0; i < player[zone].Count; i++){
					Unit randCard = (Unit)player[zone].GetIndex(i);
					if(randCard != source && randCard.hitPoints > 0 && TargetConditionCheck(randCard, condition)){ 
						cards.Add(randCard);
					}
				}
			//	cards.AddRange (player[zone].FindIndex(i));
			}
		}
		return cards;
	}

	bool TargetConditionCheck(Unit randCard, Condition condition = null)
    {
		var game = container;

		if(condition == null)
			return true;

	
		

			

			if(condition.ConditionCheck(game, randCard))
				return true;
			
			


			return false;

			

	}

	void OnValidatePlayCard (object sender, object args) {
		var playCardAction = sender as PlayCardAction;
		var card = playCardAction.card;
		var target = card.GetAspect<Target> ();
		if (target == null || (target.required == false && target.selected == null))
			return;
		var validator = args as Validator;
		
		var candidates = GetMarks (card, target.allowed);
	
		if (!candidates.Contains(target.selected)) 
			validator.Invalidate ();
		
		
	}
}