using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using TheLiquidFire.AspectContainer;
using TheLiquidFire.Notifications;

public partial class AugmentSystem : Aspect, IObserve
{
	
	
	public const string UpdateStatusNotification = "AugmentSystem.UpdateStatusNotification";
	public const string RemovePolymorphNotification = "AugmentSystem.RemovePolymorphNotification";
    public void Awake()
    {
		
		this.AddObserver (OnPerformDrawCards, Global.PerformNotification<DrawCardsAction> (), container);

		this.AddObserver (OnPerformPlayCards, Global.PerformNotification<PlayCardAction> (), container);

		this.AddObserver(OnPerformAugmentDamageAction, Global.PerformNotification<DamageAction>(), container);

		this.AddObserver (OnPerformStatusAbility, Global.PerformNotification<StatusAbilityAction> (), container);

		this.AddObserver (OnPerformDeathAugment, Global.PerformNotification<DeathAction> (), container);

		this.AddObserver (OnValidatePlayCard, Global.ValidateNotification<PlayCardAction> ());

		
    }

    public void Destroy()
    {

	
		this.RemoveObserver (OnPerformDrawCards, Global.PerformNotification<DrawCardsAction> (), container);

		this.RemoveObserver (OnPerformPlayCards, Global.PerformNotification<PlayCardAction> (), container);

		this.RemoveObserver(OnPerformAugmentDamageAction, Global.PerformNotification<DamageAction>(), container);

		this.RemoveObserver (OnPerformStatusAbility, Global.PerformNotification<StatusAbilityAction> (), container);
	
		this.RemoveObserver (OnPerformDeathAugment, Global.PerformNotification<DeathAction> (), container);

		this.RemoveObserver (OnValidatePlayCard, Global.ValidateNotification<PlayCardAction> ());
    }



	void OnValidatePlayCard (object sender, object args) {
		var playCardAction = sender as PlayCardAction;
		var validator = args as Validator;
		var aug = playCardAction.card.GetAspect<Augment>();
		if(aug == null)
			return;
		var deepFreeze = aug.statusPairs.ContainsKey("DeepFreeze");
		if(deepFreeze)
		validator.Invalidate ();
	}
	

	void OnPerformDrawCards (object sender, object args) {

		var action = args as DrawCardsAction;
	
		foreach (Card card in action.cards) {
			
		SearchForEvokeType("OnDraw", card.GetAspect<Augment>());
			
		ReduceAllEvokeTypes(card.GetAspect<Augment>());

		}
		
		
	}  

	void OnPerformDeathAugment (object sender, object args) {

		var action = args as DeathAction;
	
		
		
		SearchForEvokeType("OnDeath", action.card.GetAspect<Augment>());
			

		
		
		
	}  

	void OnPerformPlayCards (object sender, object args) {

		var action = args as PlayCardAction;
	
		
			
		SearchForEvokeType("OnPlay", action.card.GetAspect<Augment>());
			

		
		
		
	}  
	
	void OnPerformAugmentDamageAction (object sender, object args) {
		var action = args as DamageAction;
		var status = action.attachedAbility.GetAspect<Status>();

		//if(status == null)
		//	return;

		SearchForEvokeType("OnAttack", action.attachedAbility.card.GetAspect<Augment>(), action.attachedAbility);

		foreach (Card target in action.targets){

		
		
		SearchForEvokeType("OnWounded", target.GetAspect<Augment>(), action.attachedAbility);

		AddStatus(target,status,action.attachedAbility);
		

		}

	}

	public string TakeOtherInfo(string str, Ability ability){

		if(!str.Contains("info"))
			return str;

		if(str.Contains("infoability")){

		string abilityInfo = ability.userInfo.ToString();
		
		while(abilityInfo.Contains("(")){
		int f = abilityInfo.Find("(");
		int e = abilityInfo.Find(")") + 1;
		abilityInfo = abilityInfo.Remove(f, e - f);
		}

		abilityInfo = abilityInfo.Replace("infoability", "");
		

		str = str.Replace("infoability", abilityInfo);
		
		}	

		if(str.Contains("infostatus")){

		var castStatus = ability.GetAspect<Status>();

				if(castStatus != null){

				string statusInfo = castStatus.increase.ToString();

				while(statusInfo.Contains("(")){
				int f = statusInfo.Find("(");
				int e = statusInfo.Find(")") + 1;
				statusInfo = statusInfo.Remove(f, e - f);
				}

				
				statusInfo = statusInfo.Replace("infostatus", "");
				
				

				str = str.Replace("infostatus", statusInfo);

				}

		
		



		}

		str = str.Replace("||", "|");

		if(str.StartsWith("|")){
			str = str.Remove(0,1);
		}

		if(str.EndsWith("|")){
			str = str.Remove(str.Length - 1);
		}

	

		return str;

	

	}
	public int ParseAbilityInfo(string text, Card target, Ability ability){
		
		string str = text;
		str = str.ToLower().Replace("skip", "");

		int value = 0;

		var status = ability.abilityRoot.GetAspect<Status>();
		
		if(status == null){
		
		Augment augment = ability.card.GetAspect<Augment>();
			
			if(augment != null){
		    
				if(augment.statusPairs.TryGetValue("Glitch",out AbilityRoot val)){
					Status stat = val.GetAspect<Status>();
					return stat.glitchValue;
				
				}
			}
		}

		str = TakeOtherInfo(str,ability);


		var split = str.Split("|");
		foreach(var sub in split)
		value += InterpretStatusIncrease(sub, target, ability);
		return value;
	}
	public int InterpretStatusIncrease(string str, Card target, Ability ability)
    {	
		str = str.ToLower().Replace(" ", "");
		str = str.ToLower().Replace("|", "");
		str = str.ToLower().Replace("(", "");
		str = str.ToLower().Replace(")", "");
		
		if(str.Equals(""))
			return 0;

		var match = container.GetAspect<DataSystem> ().match;
		Card attachedCard = ability.card;
		var player = match.players [attachedCard.ownerIndex];
		
	
		
		

		if(ability.evokedAbility != null){
			if(str.Contains("evoke"))
				return ParseAbilityInfo(ability.evokedAbility.userInfo.ToString(), target, ability.evokedAbility);
		}

		
		
		if(str.Contains("player")){
			str = str.Replace("player", "");

				if(str.Contains("all"))
				return player.deck.Count + player.hand.Count + player.discard.Count;

				if(str.Contains("discard"))
				return player.discard.Count;

				if(str.Contains("graveyard"))
				return player.graveyard.Count;

				if(str.Contains("mana"))
				return player.mana.Available;

				str = str.Replace("name", "");
				return player.GetCardName(player,Zones.Hand, str).Count + player.GetCardName(player,Zones.Discard, str).Count + player.GetCardName(player,Zones.Deck, str).Count;
		}
		
		
		if(str.Contains("card")){
			str = str.Replace("card", "");

				Unit attachedUnit = (Unit)ability.card;

				if(str.Equals("currenthealth"))
				return attachedUnit.hitPoints;

				if(str.Equals("maxhealth"))
				return attachedUnit.maxHitPoints;
			
				if(str.Equals("missinghealth"))
				return attachedUnit.maxHitPoints - attachedUnit.hitPoints;

				if(str.Contains("status"))
				return InterpretStatusStatus(str, attachedUnit);


				str = str.Replace("name", "");
				return str.ToInt();

		}
		
		if(str.Contains("target")){
			str = str.Replace("target", "");

				Unit targetUnit = (Unit)target;

				if(str.Equals("currenthealth"))
				return targetUnit.hitPoints;

				if(str.Equals("maxhealth"))
				return targetUnit.maxHitPoints;
			
				if(str.Equals("missinghealth"))
				return targetUnit.maxHitPoints - targetUnit.hitPoints;

				if(str.Contains("status"))
				return InterpretStatusStatus(str, targetUnit);
		
		}
			
			if(int.TryParse(str, out int numericValue)){
			return numericValue;
			}else
			return 0;
	}

	public int InterpretStatusStatus(string str, Unit unit){
		str = str.Substr(6, str.Length - 1);
		//	str = str.Replace("status", "");
				Augment augment = unit.GetAspect<Augment>();
					if(augment == null)
					return 0;
		    
				if(augment.statusPairs.TryGetValue(str.ToLower(),out AbilityRoot value)){
					Status status = value.GetAspect<Status>();
					return status.value;
				
				}else
					return 0;
	}

	private void AddStatus(Card target, Status status, Ability statusAbility){
		
		if(status == null)
			return;

		var aug = target.GetAspect<Augment>();

		if(aug == null)
			aug = target.AddAspect<Augment>();

		

		var statusID = status.id;
		
		string statusMod = status.modifier.ToLower();

		string statusStr = status.increase.ToString();

		int statusInc = ParseAbilityInfo(statusStr,target, statusAbility);

		if(!aug.statusPairs.ContainsKey(statusID) && !statusMod.Contains("x") && statusInc > 0){

			DeckFactory.AddAugment(target, status, DeckFactory.Statuses[statusID]);
		
		}
		
		
		if(aug.statusPairs.TryGetValue(statusID, out AbilityRoot value)){
			
				Status originStat = null;	

				originStat = value.GetAspect<Status>();	
				
				if(!statusMod.Contains("x")){
				originStat.value += statusInc;
				}else
				originStat.value *= statusInc;
			
		if(status.name == "Glitch")
			ScrambleGlitch(originStat);
		

		StatusNotification statusNotification = new();
		statusNotification.card = target;
		statusNotification.status = originStat;
		this.PostNotification (UpdateStatusNotification, statusNotification);
		
		}
		
	}

	private void DecreaseStatus(Status status, AbilityRoot abilityRoot)
    {	
		

		if(status.statusType == StatusTypes.INNATE)
			return;
			
		var val =  Convert.ToInt32 (status.value); 
		var decr = status.decrease.ToString().ToLower();

		
		
		if(decr == "reset"){
			val = 0;
		}else
		val = Mathf.Max(val - System.Convert.ToInt32(decr), 0);
		
		status.value = val;

		if(status.name == "Glitch"){
			ScrambleGlitch(status);
		}


		if(status.value <= 0){
		
			abilityRoot.card.GetAspect<Augment>().statusPairs.Remove(status.id.ToString());

			if(status.polyCard != null){
			StatusNotification statusNotification = new();
			statusNotification.card = abilityRoot.card;
			statusNotification.status = status;
			this.PostNotification (RemovePolymorphNotification, statusNotification);
			}
				
		}
		

	}


	void OnPerformStatusAbility(object sender, object args)
	{
		var action = args as StatusAbilityAction;
		var status = action.abilityRoot.GetAspect<Status>();
		var abilityRoot = action.abilityRoot;
		var statusValue = status.value;
		var statDecr = status.decrease.ToString().ToLower();

		
		if(status.statusType == StatusTypes.STACKABLE){
			
			if(statDecr.Contains("reset"))
				statDecr = statusValue.ToString();

			
			if(statusValue < Convert.ToInt32(statDecr))	
				return;
			
		}
		

		if(status != null){
				DecreaseStatus(status,abilityRoot);
			}

		if(action.skip)
			return;

		foreach(var ability in abilityRoot.abilityChain){
		
		CheckEvokeKeyord(ability,action);
		
		if(status.statusType != StatusTypes.INNATE && status.valueTOability){
			ability.userInfo = statusValue;
		}

		
		if(status.flip){
			ability.card = action.castedAbility.card;
			ability.container = action.castedAbility.card;
		}
		
		


		
		
		var reaction = new AbilityAction(ability);
		for(int i = 0; i < ability.abilityCount; i++)
		container.AddReaction(reaction);
		
		}
	}
		void CheckEvokeKeyord(Ability ability, StatusAbilityAction action)
    	{
			if(ability.userInfo.ToString().ToLower().Contains("evoke")){
			ability.evokedAbility = action.castedAbility;
				return;
		}

		Status stat = ability.GetAspect<Status>();
		if(stat != null){
		string str = stat.increase.ToString();
			if(str.ToLower().Contains("evoke")){
				ability.evokedAbility = action.castedAbility;
				return;
			}
		}

		Condition condition = ability.GetAspect<Condition>();
		if(condition != null){
		
			if(condition.conditionInfo.ToLower().Contains("evoke") || condition.value.ToLower().Contains("evoke")){
				ability.evokedAbility = action.castedAbility;
				return;
			}
		}
		
		}

		

    private void ReduceAllEvokeTypes(Augment augment)
    {
		if(augment == null)
			return;

       foreach (var type in augment.statusPairs) {
		var stat = type.Value.GetAspect<Status>();

			if("DrawAugment" != stat.evokeType){
				
				

				

				var action = new StatusAbilityAction(type.Value);
				
				action.skip = true;

				container.AddReaction(action);

			}
	   }
    }


    void SearchForEvokeType(string evokeType, Augment aug, Ability ability = null){

		if(aug == null)
			return;

		
		Status rootStatus = null;

		if(ability != null)
			rootStatus = ability.abilityRoot.GetAspect<Status>();

		if(rootStatus != null)
			return;	
		

		foreach (var type in aug.statusPairs) {
			var stat = type.Value.GetAspect<Status>();
			
			if(evokeType == stat.evokeType){

				
				
				var action = new StatusAbilityAction(type.Value);
				
				if(ability != null){
						action.castedAbility = ability;
				}

				container.AddReaction(action);
				
				
			}

		}
			

	}



	public static Card CheckPolymorph(Card card){
		var aug = card.GetAspect<Augment>();
		
		if(aug == null)
			return null;

		
		if(aug.statusPairs.TryGetValue("polymorph", out AbilityRoot rootStatus)){
			
			Status status = rootStatus.GetAspect<Status>();
			
			string cardID = status.cardID;
			
			if(status.polyCard == null){
				status.polyCard = DeckFactory.CreateCard(cardID, aug.card.ownerIndex, null);
			}else if(status.polyCard.id != status.cardID){
				status.polyCard = DeckFactory.CreateCard(cardID, aug.card.ownerIndex, null);
			}
				return status.polyCard;

		}else
			return null;

	}


	public static bool CheckStatus(Card card, string status){

		string str = status.ToLower();
		var aug = card.GetAspect<Augment>();
		
		

		if(aug == null)
			return false;

		

		if(aug.statusPairs.TryGetValue(str, out AbilityRoot rootStatus)){

			if(rootStatus != null)
			return true;



		}

		
			return false;
	}

	private void ScrambleGlitch(Status status){
		status.glitchValue = RNGFactory.RandiRange(0, status.value);
	}


}

public class StatusNotification{
	public Card card;
	public Status status;
}