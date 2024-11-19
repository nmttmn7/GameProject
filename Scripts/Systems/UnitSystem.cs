using System.Collections;
using System.Collections.Generic;

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

		
		if(abilityRoot.GetAspect<Status>() == null){
		var poly = AugmentSystem.CheckPolymorph(action.unit);

		if(poly != null){
			abilityRoot = poly.GetAspect<AbilityRoot>();
			foreach(var ability in abilityRoot.abilityChain){
				ability.card = action.unit;
				ability.container = action.unit;
			}
		}

		}
		
		foreach(var ability in abilityRoot.abilityChain){
		var reaction = new AbilityAction(ability);
		for(int i = 0; i < ability.abilityCount; i++)
		container.AddReaction(reaction);
		}
		
		if(!AugmentSystem.CheckStatus(action.unit, "status03")){

		var match = container.GetAspect<DataSystem> ().match;
		var player = match.players [action.unit.ownerIndex];
		var discardAction = new DiscardCardsAction(player,action.unit);
		container.AddReaction(discardAction);

		}
	}
}
