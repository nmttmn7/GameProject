using System.Collections;
using System.Collections.Generic;
using Godot;

using TheLiquidFire.AspectContainer;
using TheLiquidFire.Notifications;

public class UnitSystem : Aspect, IObserve
{

	public void Awake()
	{
		this.AddObserver(OnPeformPlayCard, Global.PerformNotification<PlayCardAction>(), container);
		this.AddObserver(OnPrepareUnit, Global.PrepareNotification<UnitAction>(), container);
	}

	public void Destroy()
	{
		this.RemoveObserver(OnPeformPlayCard, Global.PerformNotification<PlayCardAction>(), container);
		this.RemoveObserver(OnPrepareUnit, Global.PrepareNotification<UnitAction>(), container);
	}

	void OnPeformPlayCard(object sender, object args)
	{
		var action = args as PlayCardAction;
		var unit = action.card as Unit;
		if (unit != null)
		{
			var reaction = new UnitAction(unit);
			container.AddReaction(reaction);
		}
	}

	void OnPrepareUnit(object sender, object args)
	{
		var action = args as UnitAction;
		var abilityRoot = action.unit.GetAspect<AbilityRoot>();

		
		if(abilityRoot.GetAspect<Stat>() == null){
	//	var poly = AugmentSystem.CheckPolymorph(action.unit);

	//	if(poly != null){
	//		abilityRoot = poly.GetAspect<AbilityRoot>();
	//		foreach(var ability in abilityRoot.abilityChain){
	//			ability.card = action.unit;
	//			ability.container = action.unit;
	//		}
	//	}

		}
		
		StatusSystem statusSystem = container.GetAspect<StatusSystem>();
		
		
		int abilityStatusINT = action.unit.GetAspect<Afflictions>().GetStatusINT("abilitychain");

		var abilityChainMIN = Mathf.Min(abilityStatusINT, abilityRoot.abilityChain.Count);
		GD.Print("COUNT " + abilityChainMIN);
		for(int i = 0; i < abilityChainMIN; i++){

		Ability ability =  abilityRoot.abilityChain[i];

		int count = statusSystem.ParseAbilityInfo(ability.abilityCount.ToString(), null, ability);
		
		for(int c = 0; c < count; c++){
		var reaction = new AbilityAction(ability);
		container.AddReaction(reaction);
		}
		
		}



		var afflictions = action.unit.GetAspect<Afflictions>();
		if(afflictions.GetStatus("anchored") == null){

		var match = container.GetAspect<DataSystem> ().match;
		var player = match.players [action.unit.ownerIndex];
		var discardAction = new DiscardCardsAction(player,action.unit);
		container.AddReaction(discardAction);

		}
	}
}
