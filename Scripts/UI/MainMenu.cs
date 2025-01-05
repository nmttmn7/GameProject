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

    private void ButtonVisibility()
    {	
		if(!FileFactory.Contains(DataManager.saveScene,"scene"))
       		continueButton.Visible = false;
    }

    private void OnNewPressed(){

		
		ClearUserFiles();
		SceneSwitcher.node.SwitchScene("res://Scenes/CollectionSelector.tscn");
		
	

	}

    private void ClearUserFiles()
    {

	FileFactory.ClearFile(DataManager.saveScene);
	
    FileFactory.ClearFile(DataManager.mapPath);
	
	FileFactory.ClearFile(DataManager.loadedLootFilePath);
	FileFactory.ClearFile(DataManager.rarityWeightFilePath);
	FileFactory.ClearFile(DataManager.lootGroupWeightFilePath);

	FileFactory.ClearFile(DataManager.playerdeckPath);

	FileFactory.ClearFile(DataManager.enemydeckPath);

    }

    private void OnContinuePressed(){

		var file = Godot.FileAccess.Open(DataManager.saveScene, Godot.FileAccess.ModeFlags.Read);
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
