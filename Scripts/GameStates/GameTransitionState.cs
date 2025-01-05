using System.Collections;
using System.Collections.Generic;
using Godot;
using TheLiquidFire.AspectContainer;

public class GameTransitionState : BaseState {
	
	public override void Enter () {
		base.Enter ();

	//	SaveFactory.SavePlayerCards(container.GetAspect<DataSystem>().match.players[0]);
	//	RNGFactory.SaveSeed();
	//	FileFactory.ClearFile(PathFactory.enemyDeckPath);
		var str = MapView.GetCurrentMapNodeData("currentMapNodeID");
		Tween tween = SceneSwitcher.node.CreateTween();

		if(str == "Boss"){
			GD.Print("Victory");
			tween.TweenInterval(2);
			tween.TweenCallback(Callable.From(() => SceneSwitcher.node.SwitchScene("res://Scenes/MainMenu.tscn")));
		}else{
			
		GD.Print("Game Transition");
		var match = container.GetAspect<DataSystem> ().match;
		
		SaveFactory.SaveDeck(DataManager.playerdeckPath, match.players[0]);
		FileFactory.ClearFile(DataManager.enemydeckPath);
		tween.TweenInterval(2);
		tween.TweenCallback(Callable.From(() => SceneSwitcher.node.SwitchScene("res://Scenes/RewardScene.tscn")));
		}
		
	}
}