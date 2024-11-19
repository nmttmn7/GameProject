using System.Collections;
using System.Collections.Generic;
using Godot;

public partial class BoardView : Node{
	public Node damageMarkPrefab;
	public List<PlayerView> playerViews = new();
    //public SetPooler cardPooler;
    //public SetPooler minionPooler;
    //public SetPooler statusPooler;

    public override void _Ready()
    {
        var match = GetTree().Root.GetNode("Main").GetNode<GameViewSystem>("GameViewSystem").container.GetMatch();
	//	var match = GetComponentInParent<GameViewSystem> ().container.GetMatch ();

		playerViews.Add(GetTree().Root.GetNode("Main").GetNode("GameViewSystem").GetNode("Board").GetNode<PlayerView>("AllyPlayerView"));
		playerViews.Add(GetTree().Root.GetNode("Main").GetNode("GameViewSystem").GetNode("Board").GetNode<PlayerView>("EnemyPlayerView"));	

		for (int i = 0; i < match.players.Count; ++i) {
			playerViews [i].SetPlayer (match.players[i]);
		}
		
    }


	public Node GetMatch (Card card) {
		var playerView = playerViews [card.ownerIndex];
		return playerView.GetMatch (card);
	}

}