using System.Collections;
using System.Collections.Generic;

using TheLiquidFire.Notifications;
using TheLiquidFire.AspectContainer;
using TheLiquidFire.Extensions;
using Godot;


public partial class UIView : Node {

	

	

	RichTextLabel playerHealthLabel;
	RichTextLabel enemyHealthLabel;
	RichTextLabel enemyDrawLabel;
	public override void _Ready()
	{

		this.AddObserver (OnPerformDeathCard, Global.PerformNotification<DeathAction> ());

		this.AddObserver (OnPlayerDrawIndexValueChangedNotification, PlayerSystem.ValueChangedNotification);
		
		
		playerHealthLabel = GetTree().Root.GetNode("Main").GetNode("PlayerHealthConstruct").GetChild<RichTextLabel>(1);
		enemyHealthLabel = GetTree().Root.GetNode("Main").GetNode("EnemyHealthConstruct").GetChild<RichTextLabel>(1);

		
		enemyDrawLabel = GetTree().Root.GetNode("Main").GetNode("EnemyDrawLabel").GetChild<RichTextLabel>(0);
	}

	public override void _ExitTree()
    {	

		this.RemoveObserver (OnPlayerDrawIndexValueChangedNotification, PlayerSystem.ValueChangedNotification);

		this.RemoveObserver (OnPerformDeathCard, Global.PerformNotification<DeathAction> ());
	}


	void OnPlayerDrawIndexValueChangedNotification (object sender, object args) {
		var index = System.Convert.ToInt32(args);
		
		GD.Print("index " + index);
		enemyDrawLabel.Text = "[center]Draw: " + index.ToString() + " ";
		enemyDrawLabel.Text += "Round: " + PlayerSystem.round;
	}


	public void StartUI(IContainer container){
		var match = container.GetMatch ();
		Player player0 = match.players[0];
		Player player1 = match.players[1];
		ChangePlayerHealth(player0);
		ChangePlayerHealth(player1);
	}

	void OnPerformDeathCard(object sender, object args){
		var action = args as DeathAction;
		ChangePlayerHealth(action.player);
	}

	void ChangePlayerHealth(Player player){
		int health = player.deck.Count + player.discard.Count + player.hand.Count;
		if(player.index == 0){
		playerHealthLabel.Text = "[center]" + health.ToString();
		}else
		enemyHealthLabel.Text = "[center]" + health.ToString();
	}

	void ChangeDrawAmount(Player player){
		int health = player.deck.Count + player.discard.Count + player.hand.Count;
		if(player.index == 0){
		playerHealthLabel.Text = "[center]" + health.ToString();
		}else
		enemyHealthLabel.Text = "[center]" + health.ToString();
	}




	
}