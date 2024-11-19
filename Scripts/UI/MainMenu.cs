using Godot;
using System;
using System.Collections.Generic;

public partial class MainMenu : Node
{
	[Export] Button newButton;
	[Export] Button continueButton;
	[Export] Button optionsButton;
	[Export] Button quitButton;
	public override void _Ready(){
		
		ButtonVisibility();



	}

	public static string saveScene = "res://UserData/Scene/SaveScene.txt";
    private void ButtonVisibility()
    {	
		if(!FileFactory.Contains(saveScene,"scene"))
       		continueButton.Visible = false;
    }

    private void OnNewPressed(){

		
		ClearUserFiles();
		SceneSwitcher.node.SwitchScene("res://Scenes/CollectionSelector.tscn");
		
	

	}

    private void ClearUserFiles()
    {

	FileFactory.ClearFile(saveScene);
	
    FileFactory.ClearFile(MapView.mapPath);
	
	FileFactory.ClearFile(RewardView.loadedLootFilePath);
	FileFactory.ClearFile(RewardView.rarityWeightFilePath);
	FileFactory.ClearFile(RewardView.lootGroupWeightFilePath);

	FileFactory.ClearFile(GameViewSystem.deckFilePath);

	FileFactory.ClearFile("res://UserData/Cards/Enemy/Deck.txt");

    }

    private void OnContinuePressed(){

		var file = Godot.FileAccess.Open(saveScene, Godot.FileAccess.ModeFlags.Read);
		var fileText = file.GetAsText();
		var contents = MiniJSON.Json.Deserialize (fileText) as Dictionary<string, object>;
		file.Close();
		SaveFactory.LoadDisposition();
		string scene = "";

		var array = (Dictionary<string, object>)contents;
		
		scene = (string)array["scene"];
		
		RNGFactory.LoadSeed();
		SceneSwitcher.node.SwitchScene(scene);
		
		
	}


	private void OnOptionsPressed(){
		
	}

	private void OnQuitPressed(){
		GetTree().Quit();
	}
}
