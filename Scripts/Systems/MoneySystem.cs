using Godot;
using System;
using TheLiquidFire.AspectContainer;
using TheLiquidFire.Notifications;

public class MoneySystem : Aspect, IObserve {

	private int moneyDispurse = 100;
	public void Awake () {
		this.AddObserver(OnPeformPlayCard, Global.PerformNotification<PlayCardAction>(), container);
		this.AddObserver (OnPerformDeath, Global.PerformNotification<DeathAction> (), container);
	}

	public void Destroy () {
		this.AddObserver(OnPeformPlayCard, Global.PerformNotification<PlayCardAction>(), container);
		this.RemoveObserver (OnPerformDeath, Global.PerformNotification<DeathAction> (), container);
	}

	public Unit playedUnit;

	void OnPeformPlayCard(object sender, object args)
	{
		var action = args as PlayCardAction;
		var unit = action.card as Unit;
		if (unit != null)
		{
			playedUnit =  unit;
		}
	}

	void OnPerformDeath (object sender, object args) {

		var action = args as DeathAction;
		
		if(playedUnit == null)
			return;
			
		var deadIndex =	action.card.ownerIndex;
		int index = playedUnit.ownerIndex;

		if(deadIndex == index)
			return;

		int active = (int)(moneyDispurse * .60f);
		int passive = (int)(moneyDispurse * .40f);
		
		var match = container.GetMatch ();
		
		playedUnit.money += active;

		Player player = match.players[index];
		foreach (Unit card in player[Zones.Hand]) {
				card.money += passive;
			}

		
	}

}