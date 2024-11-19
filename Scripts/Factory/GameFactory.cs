using System.Collections;
using System.Collections.Generic;

using TheLiquidFire.AspectContainer;

public static class GameFactory {

	public static Container Create () {
		Container game = new Container ();

		//Seed
		//game.AddAspect<SeedSystem>();
		
		// Add Systems
		game.AddAspect<AbilitySystem> ();
		
		game.AddAspect<ActionSystem> ();
	//	game.AddAspect<AttackSystem> ();
		game.AddAspect<CardCreationSystem>();
		game.AddAspect<CardSystem> ();
	//	game.AddAspect<CombatantSystem> ();
	//	game.AddAspect<CompanionSystem>();
		game.AddAspect<DataSystem> ();
		game.AddAspect<DeathSystem> ();
		game.AddAspect<DestructableSystem> ();
		game.AddAspect<EnemySystem> ();
		game.AddAspect<ManaSystem> ();
		game.AddAspect<MatchSystem> ();
	//	game.AddAspect<MinionSystem> ();
	
		game.AddAspect<PlayerSystem> ();
		game.AddAspect<UnitSystem> ();
		
		game.AddAspect<TargetSystem> ();
	//	game.AddAspect<TauntSystem> ();

		game.AddAspect<AugmentSystem>();
		game.AddAspect<VictorySystem> ();
		
		game.AddAspect<MoneySystem>();
		
	
		
		// Add Other
		game.AddAspect<StateMachine> ();
		game.AddAspect<GlobalGameState> (); 
		

		return game;
	}
}