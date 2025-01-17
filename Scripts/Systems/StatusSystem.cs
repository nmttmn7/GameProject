using Godot;
using GodotPlugins.Game;
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
	public const string DecreaseStatusNotification = "StatusSystem.DecreaseStatusNotification";
    public void Awake()
    {
        this.AddObserver(OnPerformDamageAction, Global.PerformNotification<DamageAction>(), container);
    //    this.AddObserver (OnPerformStatusAbility, Global.PerformNotification<StatusAbilityAction> (), container);
		this.AddObserver (OnPerformStatusAbility, Global.PerformNotification<AbilityAction> (), container);
        this.AddObserver (OnPerformDrawCardsAction, Global.PerformNotification<DrawCardsAction> (), container);
        this.AddObserver (OnPerformDeathAction, Global.PerformNotification<DeathAction> (), container);
        this.AddObserver (OnPerformPlayCardAction, Global.PerformNotification<PlayCardAction> (), container);
    }

    public void Destroy()
    {
        this.RemoveObserver(OnPerformDamageAction, Global.PerformNotification<DamageAction>(), container);
        //this.RemoveObserver (OnPerformStatusAbility, Global.PerformNotification<StatusAbilityAction> (), container);
		this.AddObserver (OnPerformStatusAbility, Global.PerformNotification<AbilityAction> (), container);
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

		
		if(card != null && !action.attachedAbility.GetInfo().Contains("skip"))
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

			if(!status.evokeType.Contains("OnDraw")){
				
				

				
				
				DecreaseStatus(status, status.id, status.decrease,3);

			}
	   }
    }
	
    public void DecreaseStatus(Status status, string id, int decrementAmount, int decrement = 2)
    {	
		if(status == null || status.id != id)
			return;

		
		if(decrement == 1){
	
	
        status.value = Mathf.Max(status.value - decrementAmount, 0);
		
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
		
		
        status.value = Mathf.Max(status.value - decrementAmount, 0);
	

		if(status.value <= 0)
            status.container.GetAspect<Afflictions>().statusPairs.Remove(status.id);

        }else{

        if(status.statusType == StatusTypes.INNATE)
			return; 

		
        status.value = Mathf.Max(status.value - decrementAmount, 0);
		

		if(status.value <= 0)
            status.container.GetAspect<Afflictions>().statusPairs.Remove(status.id);


        }
		

		Card card = status.container as Card;
		DecreaseStatusNotify(card, status);
			
	}
    
 	void OnPerformStatusAbility(object sender, object args)
	{
		var action = args as AbilityAction;
		var status = action.ability.abilityRoot.container as Status;

		if(status == null)
			return;

		

		
			
		
		DecreaseStatus(status, status.id, status.decrease);
			

	}

/*
    void CheckEvokeKeyord(Ability ability, StatusAbilityAction action)
    	{



		if(ability.GetInfo().Contains("evoke") || ability.GetAspect<ITargetSelector>().ToString() == "EvokeTarget"){
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
		
		} */
 
    void SearchForEvokeType(string evokeType, Card card, Ability castedAbility = null){

        if(card == null)
       	 return;

        Afflictions targetAfflictions = card.GetAspect<Afflictions>();
	

		
		foreach(var s in targetAfflictions.statusPairs){
			Status stat = s.Value;
			if(stat.evokeType.Contains(evokeType) || ParseStackable(stat.evokeType,stat.value)){
			foreach(var pair in stat.abilityRoot.abilityChain){

				

				if(castedAbility != null)
					pair.evokedAbility = castedAbility;

				
				int count = this.ParseAbilityInfo(pair.abilityCount.ToString(), null, pair);

				for(int i = 0; i < count; i++){
				
				var action = new AbilityAction(pair);
				container.AddReaction(action);
				}

				

			}
			}

			
		}


	}


    public bool ParseStackable(string str, int statusVALUE){
      
            var f = str.Find("{") + 1;
            
            var e = str.Find("}");
           
            if(f == 0 || e == -1)
                return false;


            string temp = str.Substr(f,e - f);
            


            f = temp.Find("≤");
            if(f == -1)
            f = temp.Find("≥");

				
            string comparator = temp.Substring(f);
            
            temp = temp.Remove(f);

            

            if(comparator.Equals("≤") && Int32.Parse(temp) <= statusVALUE)
                return true;
            else if(comparator.Equals("≥") && Int32.Parse(temp) >= statusVALUE)
                return true;

                return false;
    }

    public void InitializeCard(Card card, Dictionary<string, object> data){
        if (data.ContainsKey("afflictions") == false)
			return;
		
		var afflictionsData = (List<object>)data ["afflictions"];

		var castedAbility = card.GetAspect<AbilityRoot>().abilityChain[0];

		foreach (object entry in afflictionsData) {
            
			StatusData statusData = new();
            statusData.data = (Dictionary<string, object>)entry;
			AddStatus(card, statusData, castedAbility);
			
			
		}
    }


	 public void CreateCard(Card card, Dictionary<string, object> data, Card attached){
        if (data.ContainsKey("afflictions") == false)
			return;

		
		
		var afflictionsData = (List<object>)data ["afflictions"];

		var castedAbility = card.GetAspect<AbilityRoot>().abilityChain[0];
		
		foreach (object entry in afflictionsData) {
            
			StatusData statusData = new();
            statusData.data = (Dictionary<string, object>)entry;
			AddStatus(card, statusData,castedAbility,false);
			
			
		}

		
    }

	public void ShareAfflictions(Card original, Card spawn){

		if(original.GetAspect<Afflictions>().GetStatus("cultist") == null) return;

		var oA = original.GetAspect<Afflictions>();

		var sA = spawn.GetAspect<Afflictions>();

		foreach(var pair in oA.statusPairs)
			pair.Value.AddConnectedCard(spawn);

		sA.statusPairs = oA.statusPairs;

		
		
	}
	
    public void AddStatus(Card target, StatusData s, Ability castedAbility, bool initialized = true){



        if(s == null)
            return;

		Afflictions aff = target.GetAspect<Afflictions>();

		if(!initialized && aff.GetStatus("x") != null)
			return;

	
         Dictionary<string, object> statusData = s.data;

        

       


        string id = (string)statusData["id"];
		id = id.ToLower();


        if(!aff.statusPairs.ContainsKey(id) && initialized && !s.data.ContainsKey("modifier"))
            AddGlobalStatus(target, id, castedAbility);



        EditCurrentStatus(target, statusData, castedAbility);


    }
	


    

	private void AddGlobalStatus(Card target, string id, Ability castedAbility){
		

		Afflictions aff = target.GetAspect<Afflictions>();


		

		if(id == "health"){

		Health health = new();

       	health.Load(DeckFactory.Afflictions[id],target, castedAbility, this);
        
        health.AddConnectedCard(target);
        aff.statusPairs.Add(id, health);

		return;

		}else if(id == "token"){

		Token token = new();

       	token.Load(DeckFactory.Afflictions[id],target, castedAbility, this);
        
        token.AddConnectedCard(target);
        aff.statusPairs.Add(id, token);

		return;

		}



        Status status = new();

        status.Load(DeckFactory.Afflictions[id],target, castedAbility, this);
        
        status.AddConnectedCard(target);
        aff.statusPairs.Add(id, status);

   
	
    
    }


	private void EditCurrentStatus(Card target, Dictionary<string, object> statusData, Ability castedAbility)
    {   

        string statusID = (string)statusData["id"];
		statusID = statusID.ToLower();

        Afflictions aff = target.GetAspect<Afflictions>();
		Status status = new();

        if(aff.statusPairs.TryGetValue(statusID, out Status s)){
            status = s;
            status.Load(statusData,target,castedAbility,this);

			if(statusData.ContainsKey("incr")){
				if(statusData.ContainsKey("modifier"))
					status.ChangeValue((string)statusData ["incr"], (string)statusData ["modifier"],target,castedAbility,this);
				else
					status.ChangeValue((string)statusData ["incr"], "+",target,castedAbility,this);
			}


			status.Override(aff,statusID,statusData);
			
          	SearchForEvokeType("MEOW", target);
            //SearchForEvokeType(status.value, target.GetAspect<Afflictions>(), castedAbility);
        
		}else if(!statusData.ContainsKey("modifier")){

			
			status.Load(statusData,target,castedAbility,this);
			aff.statusPairs.Add(status.id, s);
		}
       
        UpdateStatus(target,status);
    }

	
	public void DecreaseStatusNotify(Card target, Status status){
		
		if(target == null ||  status == null)
			return;

		UpdateStatusNotification statusNotification = new(target,status);
		this.PostNotification (DecreaseStatusNotification, statusNotification);
	}
    
	public void UpdateStatus(Card target, Status status){
		
		if(target == null ||  status == null)
			return;

		UpdateStatusNotification statusNotification = new(target,status);
		this.PostNotification (UpdateStatusNotification, statusNotification);
	}
    

	#region DataReader
	public string TakeOtherInfo(string str, Ability ability){

		if(!str.Contains("info"))
			return str;

		if(str.Contains("infoability")){

		string abilityInfo = ability.GetInfo();
		
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
				return ParseAbilityInfo(ability.evokedAbility.GetInfo(), target, ability.evokedAbility);
		

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
				return ability.card.GetAspect<Afflictions>().GetStatusINT("health");

				if(str.Equals("maxhealth")){
				var health = ability.card.GetAspect<Afflictions>().GetStatus("health") as Health;
				return health.maxHealth;
				}

				if(str.Equals("missinghealth")){
				var health = ability.card.GetAspect<Afflictions>().GetStatus("health") as Health;
				return health.maxHealth - health.value;
				}

				if(str.Equals("mana"))
				return ability.card.cost;

				if(str.Contains("status"))
				return InterpretStatusStatus(str, ability.card);


				str = str.Replace("name", "");
				return str.ToInt();

		}
		
		if(str.Contains("target") && target != null){
			str = str.Replace("target", "");


				if(str.Equals("currenthealth"))
				return target.GetAspect<Afflictions>().GetStatusINT("health");

				if(str.Equals("maxhealth")){
				var health = target.GetAspect<Afflictions>().GetStatus("health") as Health;
				return health.maxHealth;
				}

				if(str.Equals("missinghealth")){
				var health = target.GetAspect<Afflictions>().GetStatus("health") as Health;
				return health.maxHealth - health.value;
				}

				if(str.Equals("mana"))
				return target.cost;

				if(str.Contains("status"))
				return InterpretStatusStatus(str, target);
		
		}
			
			if(int.TryParse(str, out int numericValue)){
			return numericValue;
			}else
			return 0;
	}

	public int InterpretStatusStatus(string str, Card card){
		str = str.Substr(6, str.Length - 1);
		
        return card.GetAspect<Afflictions>().GetStatusINT(str);
	}

	#endregion
}


public class UpdateStatusNotification{
	public Card card;
	public Status status;

	public UpdateStatusNotification(Card target, Status status){
		this.card = target;
		this.status = status;

	}
}