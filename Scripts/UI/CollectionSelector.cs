using Godot;
using System;

public partial class CollectionSelector : Node
{
	[Export] TextureButton firstButton;
	[Export] TextureButton secondButton;
	[Export] TextureButton thirdButton;
	[Export] TextureButton devButton;
	public override void _Ready(){
		
		ButtonVisibility();

	}

    private void ButtonVisibility()
    {
       devButton.Visible = false;
    }

    private void OnFirstPressed(){
		
		SceneSwitcher.node.dispositions.Add("D1");
		SaveFactory.SaveDisposition();
		SceneSwitcher.node.SwitchScene("res://Scenes/MapScene.tscn");
		
	}

	private void OnSecondPressed(){
		
		SceneSwitcher.node.dispositions.Add("D2");
		SaveFactory.SaveDisposition();
		SceneSwitcher.node.SwitchScene("res://Scenes/MapScene.tscn");
	}

	private void OnThirdPressed(){
		
		SceneSwitcher.node.dispositions.Add("D3");
		SaveFactory.SaveDisposition();
		SceneSwitcher.node.SwitchScene("res://Scenes/MapScene.tscn");
	}

	private void OnDevPressed(){
		
		SceneSwitcher.node.dispositions.Add("dev");
		SaveFactory.SaveDisposition();
		SceneSwitcher.node.SwitchScene("res://Scenes/MapScene.tscn");
	}

}
