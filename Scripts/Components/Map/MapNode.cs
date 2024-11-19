using Godot;
using System;
using System.Collections.Generic;

public partial class MapNode : Node
{
	[Export] public AnimatedSprite2D animatedSprite2D;
	[Export] public Node2D MapDestinationConstruct;
	[Export] public AnimatedSprite2D spriteAnima;
	[Export] public Button mapButton;
	public int depth;
	public Vector2 mapPosition;
	public string path;
	public List<MapNode> mapNodes = new List<MapNode>();
	public int jaggedX;
	public int jaggedY;
	public bool connected;
	public string mapID;
	
	
	
	public void OnButtonPressed(){

		if(path.Length > 0){
					var mapConstruct = GetTree().Root.GetNode<MapView>("MapScene");
					if(mapConstruct.currentNode == null && depth == 0){
						
				
						mapConstruct.currentNode = this;
						SaveFactory.SavePlayerMap(mapConstruct);
						SceneSwitcher.node.SwitchScene(path);

				}else if(mapConstruct.currentNode != null){
					if(mapConstruct.currentNode.mapNodes.Contains(this)){
					
				
					mapConstruct.currentNode = this;
					SaveFactory.SavePlayerMap(mapConstruct);
					SceneSwitcher.node.SwitchScene(path);
				}
				}

				}else{
						
				}
	}
	public void OnMouseEnter(){
		Tween tween = CreateTween();
		tween.TweenProperty(spriteAnima, "scale", new Vector2(1, 1), 0.5f);
	}

	public void OnMouseExit(){
		Tween tween = CreateTween();
		tween.TweenProperty(spriteAnima, "scale", new Vector2(.6f, .6f), 0.5f);
	}

	public string Save(){
		string text = "";
			text += "\n\"depth\": " + "\"" + depth + "\","; 
			text += "\n\"mapPosition\": " + "\"" + mapPosition + "\","; 
			text += "\n\"path\": " + "\"" + path + "\","; 
			text += "\n\"mapID\": " + "\"" + mapID + "\","; 
			text += "\n\"connectedNodes\": {"; 
			for(int i = 0; i < mapNodes.Count; i++){
				text += "\n\"connectedX" + i + "\": " + "\"" + mapNodes[i].jaggedX + "\","; 
			//	text += "\n\"connectedY\": " + "\"" + mapNodes[i].jaggedY + "\","; 
			}
			text += "\n }";
			
		return text;
	}
   
}
