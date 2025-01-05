using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using TheLiquidFire.AspectContainer;
using TheLiquidFire.Notifications;

public partial class StatusSystem : Aspect, IObserve
{   


    public const string UpdateStatusNotification = "StatusSystem.UpdateStatusNotification";
    public void Awake()
    {
        this.AddObserver(OnPerformDamageAction, Global.PerformNotification<DamageAction>(), container);
        this.AddObserver (OnPerformStatusAbility, Global.PerformNotification<StatusAbilityAction> (), container);
        this.AddObserver (OnPerformDrawCardsAction, Global.PerformNotification<DrawCardsAction> (), container);
        this.AddObserver (OnPerformDeathAction, Global.PerformNotification<DeathAction> (), container);
        this.AddObserver (OnPerformPlayCardAction, Global.PerformNotification<PlayCardAction> (), container);
    }

    public void Destroy()
    {
        this.RemoveObserver(OnPerformDamageAction, Global.PerformNotification<DamageAction>(), container);
        this.RemoveObserver (OnPerformStatusAbility, Global.PerformNotification<StatusAbilityAction> (), container);
        this.RemoveObserver (OnPerformDrawCardsAction, Global.PerformNotification<DrawCardsAction> (), container);
        this.RemoveObserver (OnPerformDeathAction, Global.PerformNotification<DeathAction> (), container);
        this.RemoveObserver (OnPerformPlayCardAction, Global.PerformNotification<PlayCardAction> (), container);
    }

    #region ActionCalls


    void OnPerformDrawCardsAction (object sender, object args) {

		var action = args as DrawCardsAction;

       // var status = action.attachedAbility.abilityRoot.container as Status;

		foreach (Card card in action.cards) {
		
        //  if(status == null)
		SearchForEvokeType("OnDraw", card);
			if(!action.createdCard)
		ReduceAllEvokeTypes(card.GetAspect<Afflictions>());

		}
		
		
	}


    void OnPerformDamageAction (object sender, object args) {
		var action = args as DamageAction;

		var statusData = action.attachedAbility.GetAspect<StatusData>();

        var card = action.attachedAbility.abilityRoot.container as Card;



		SearchForEvokeType("OnAttack", card, action.attachedAbility);

		foreach (Card target in action.targets){

		
		if(card != null && !action.attachedAbility.userInfo.ToString().Contains("skip"))
		SearchForEvokeType("OnWounded", target, action.attachedAbility);

		AddStatus(target, statusData, action.attachedAbility);
		

		}

	}

    void OnPerformDeathAction (object sender, object args) {

		var action = args as DeathAction;
	
		
		
		SearchForEvokeType("OnDeath", action.card);
			

		
		
		
	}  

	void OnPerformPlayCardAction (object sender, object args) {

		var action = args as PlayCardAction;
	
		
    
		SearchForEvokeType("OnPlay", action.card);
				
		
		
	}  

    

    #endregion 

    private void ReduceAllEvokeTypes(Afflictions afflictions)
    {
		if(afflictions == null)
			return;

       foreach (var type in afflictions.statusPairs) {
		var status = type.Value;

			if("OnDraw" != status.evokeType){
				
				

				

				DecreaseStatus(status,status.decrease,3);

			}
	   }
    }

    public void DecreaseStatus(Status status, int decrementAmount, int decrement = 2)
    {	
		if(status == null)
			return;


		if(decrement == 1){
	
	
        status.value = Mathf.Max(status.value - status.decrease, 0);
		
		if(status.statusType == StatusTypes.INNATE)
			status.value = 0; 


		if(status.value <= 0)
            status.container.GetAspect<Afflictions>().statusPairs.Remove(status.id);

        }else if(decrement == 2){

        //ReduceAllStatusOnDraw
	
        if(status.statusType == StatusTypes.INNATE)
			return; 

        
        if(status.statusType == StatusTypes.STACKABLE)
            status.value = 0;

        status.value = Mathf.Max(status.value - status.decrease, 0);
		

		if(status.value <= 0)
            status.container.GetAspect<Afflictions>().statusPairs.Remove(status.id);

        }else{

        if(status.statusType == StatusTypes.INNATE)
			return; 


        status.value = Mathf.Max(status.value - status.decrease, 0);
		

		if(status.value <= 0)
            status.container.GetAspect<Afflictions>().statusPairs.Remove(status.id);


        }

			
	}
    

    void OnPerformStatusAbility(object sender, object args)
	{
		var action = args as StatusAbilityAction;
		var status = action.status;
		var statusValue = status.value;

		
		if(status.statusType == StatusTypes.STACKABLE && status.value < status.decrease)
				return;
			

		DecreaseStatus(status, status.decrease);
			
    

		foreach(var ability in status.abilityRoot.abilityChain){
		

		ability.evokedAbility = action.castedAbility;
		
	
		
		//if(status.flip)
		//	ability.card = action.castedAbility.card;
			
		
		
		
        ability.userInfo = SetStatusValue(ability.userInfo,status,statusValue);

		
		var reaction = new AbilityAction(ability);

        StatusSystem statusSystem = container.GetAspect<StatusSystem>();
		
		int count = statusSystem.ParseAbilityInfo(ability.abilityCount.ToString(), null, ability);

		for(int i = 0; i < count; i++)
		container.AddReaction(reaction);
		
		}
	}

    object SetStatusValue(object obj, Status status, int originalStatusValue)
    {
        
        string str = "cardstatus" + status.id;

        string userInfoString = obj.ToString();

        userInfoString =  userInfoString.Replace(str, originalStatusValue.ToString());

        obj = userInfoString;
         
        return obj;
       
    }

    void CheckEvokeKeyord(Ability ability, StatusAbilityAction action)
    	{



		if(ability.userInfo.ToString().ToLower().Contains("evoke") || ability.GetAspect<ITargetSelector>().ToString() == "EvokeTarget"){
			ability.evokedAbility = action.castedAbility;
				return;
		}






	//	Stat stat = (Status)ability.container;
	//	if(stat != null){
	//	string str = stat.increase.ToString();
	//		if(str.ToLower().Contains("evoke")){
	//			ability.evokedAbility = action.castedAbility;
	//			return;
	//		}
	//	}

		Condition condition = ability.GetAspect<Condition>();
		if(condition != null){
		
			if(condition.conditionInfo.ToLower().Contains("evoke") || condition.value.ToLower().Contains("evoke")){
				ability.evokedAbility = action.castedAbility;
				return;
			}
		}
		
		}

    void SearchForEvokeType(string evokeType, Card card, Ability ability = null){
        if(card == null)
        return;

        Afflictions targetAfflictions = card.GetAspect<Afflictions>();
		if(targetAfflictions == null)
			return;

		
		

		foreach (var type in targetAfflictions.statusPairs) {
			var status = type.Value;
			if(status.evokeType.Contains(evokeType)){

				
				
				var action = new StatusAbilityAction(status);
				
				if(ability != null)
						action.castedAbility = ability;
				

				container.AddReaction(action);
				
				
			}

		}
			

	}

    void SearchForEvokeType(int evokeType, Afflictions targetAfflictions, Ability ability = null){

		if(targetAfflictions == null)
			return;

		
		

		foreach (var type in targetAfflictions.statusPairs) {
			var status = type.Value;
          
          
			if(ParseStackable(status.evokeType,status.value)){

				
				
				var action = new StatusAbilityAction(status);
				
				if(ability != null)
						action.castedAbility = ability;
				

				container.AddReaction(action);
				
				
			}

            

		}
			

	}

    public bool ParseStackable(string str, int statusVALUE){
      
            var f = str.Find("{") + 1;
            
            var e = str.Find("}");
           
            if(f == 0 || e == -1)
                return false;


            string temp = str.Substr(f,e - f);
            


            f = temp.Find("<");
            if(f == -1)
            f = temp.Find(">");

            string comparator = temp.Substring(f);
            
            temp = temp.Remove(f);

            

            if(comparator.Equals("<=") && Int32.Parse(temp) <= statusVALUE)
                return true;
            else if(comparator.Equals(">=") && Int32.Parse(temp) >= statusVALUE)
                return true;

                return false;
    }

    public void InitializeCard(Card card, Dictionary<string, object> data){
        if (data.ContainsKey("afflictions") == false)
			return;

		var afflictions = card.AddAspect<Afflictions> ();
		
		var afflictionsData = (List<object>)data ["afflictions"];

		
		foreach (object entry in afflictionsData) {
            
			StatusData statusData = new();
            statusData.data = (Dictionary<string, object>)entry;
			AddStatus(card, statusData);
			
			
		}
    }


	 public void MM(Card card, Dictionary<string, object> data){
        if (data.ContainsKey("afflictions") == false)
			return;

		var afflictions = card.AddAspect<Afflictions> ();
		
		var afflictionsData = (List<object>)data ["afflictions"];

		
		foreach (object entry in afflictionsData) {
            
			StatusData statusData = new();
            statusData.data = (Dictionary<string, object>)entry;
			AddStatus(card, statusData,null,false);
			
			
		}
    }
	
    private void AddStatus(Card target, StatusData s, Ability castedAbility = null, bool se = true){



        if(s == null)
            return;


         Dictionary<string, object> statusData = s.data;

        Afflictions aff = target.GetAspect<Afflictions>();

        if(aff == null)
            aff = target.AddAspect<Afflictions>();


        string id = (string)statusData["id"];
		id = id.ToLower();


        if(!aff.statusPairs.ContainsKey(id) && se && !s.data.ContainsKey("modifier"))
            AddGlobalStatus(target, id);



        EditCurrentStatus(target, statusData, castedAbility);

		if(GetStatus(target,id) != null){
        UpdateStatusNotification statusNotification = new();
		statusNotification.card = target;
		statusNotification.status = GetStatus(target, id);
		this.PostNotification (UpdateStatusNotification, statusNotification);
		}

    }

    private void EditCurrentStatus(Card target, Dictionary<string, object> statusData, Ability castedAbility)
    {   

        string statusID = (string)statusData["id"];
		statusID = statusID.ToLower();

        Afflictions aff = target.GetAspect<Afflictions>();

        if(aff.statusPairs.TryGetValue(statusID, out Status status)){
            
            status.Load(statusData,target,castedAbility,this);
           
           
            SearchForEvokeType(status.value, target.GetAspect<Afflictions>(), castedAbility);
        
		}else if(!statusData.ContainsKey("modifier")){

			Status s = new();
			s.Load(statusData,target,castedAbility,this);
			aff.statusPairs.Add(s.id, s);
		}
       
        
    }

    public static Status GetStatus(Card target, string id){
        string str = id.ToLower();
		var aff = target.GetAspect<Afflictions>();
		
		

		if(aff == null)
			return null;

		

		if(aff.statusPairs.TryGetValue(str, out Status status)){

			if(status != null)
			return status;



		}

		
			return null;
    }


    public static int GetStatusValue(Card target, string id){
        string str = id.ToLower();
		var aff = target.GetAspect<Afflictions>();
		
		

		if(aff == null)
			return 0;

		

		if(aff.statusPairs.TryGetValue(str, out Status status)){

			if(status != null)
			return status.value;



		}

		
			return 0;
    }

    

	private void AddGlobalStatus(Card target, string id){
		

		Afflictions aff = target.GetAspect<Afflictions>();

        Status status = new();

        status.Load(DeckFactory.Statuses[id],target);
        
        
        aff.statusPairs.Add(id, status);

   
	
    
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

		var castStatus = ability.GetAspect<Stat>();

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
		
	
		
		

		if(ability.evokedAbility != null && str.Contains("evoke"))
				return ParseAbilityInfo(ability.evokedAbility.userInfo.ToString(), target, ability.evokedAbility);
		

		if(str.Contains("abilitychainposition")){
			return ability.chainPosition;
		}

		if(str.Contains("abilitychainmax")){
			return ability.abilityRoot.abilityChain.Count+1;
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
		
		if(str.Contains("target") && target != null){
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
		
        return GetStatusValue(unit, str);
	}


}


public class UpdateStatusNotification{
	public Card card;
	public Status status;
}