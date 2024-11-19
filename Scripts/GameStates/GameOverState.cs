using System.Collections;
using System.Collections.Generic;
using Godot;
using TheLiquidFire.AspectContainer;

public class GameOverState : BaseState {
	
	public override void Enter () {
		base.Enter ();
		GD.Print("Game Over");
		
	//	FileFactory.ClearFile(MapGenerator.mapPath);
	//	RNGFactory.ClearSeedFile();
	//	FileFactory.ClearFile(PathFactory.enemyDeckPath);
	//	FileFactory.ClearFile(PathFactory.playerDeckPath);
	//	FileFactory.ClearFile(PathFactory.playerGraveyardPath);

		Tween tween = SceneSwitcher.node.CreateTween();
		tween.TweenInterval(2);
		tween.TweenCallback(Callable.From(() => SceneSwitcher.node.SwitchScene("res://Constructs/MainMenu.tscn")));
	//	container.ChangeState<PlayerIdleState> ();
	//	SceneSwitcher.node.SwitchScene("res://Constructs/MainMenu.tscn");
		
		
	}
}