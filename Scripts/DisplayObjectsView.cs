using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
//using TheLiquidFire.Extensions;

public partial class DisplayObjectsView : Node
{
	
	float topBounds;
	float botBounds;

	
	
	public void DeleteAllOtherObjects(Node list, Node kept){
		var children = list.GetChildren();

		foreach(var child in children){

			if(child != kept)
			child.QueueFree();

		}
	}
	
	public IEnumerator RewardObjectSelected(Node2D node2D){

		Tween tween = CreateTween();
		var position = node2D.Position;
		position.Y -= -20;
		tween.TweenProperty(node2D, "position", position, .05f).SetEase(Tween.EaseType.Out);
		while (tween != null && tween.IsRunning())
			yield return null;
	}
	public IEnumerator LayoutObjects(Node2D node, int elementIndent, bool animated = true)
	{	
		var children = node.GetChildren();

		float x = 10.5f * children.Count;
		
		if(children.Count == 0)
			yield break;


		if(children.Count < elementIndent)
			elementIndent = children.Count;

		var overlapY = 200f;
		var overlapX = 250f - x;
		//145
		var width = elementIndent * overlapX;
		var xPos = -(width / 2f);
		var yPos = 0f;

		xPos += overlapX / 2;
		
		var duration = animated ? 0.25f : 0;
		Tween tween = CreateTween();
		for (int i = 0; i < children.Count; ++i)
		{


		if(i % elementIndent == 0 && i > 0){
		 yPos -= overlapY;
		 xPos = -(width / 2f);
		 xPos += overlapX / 2;
		}
		
			var position = new Vector2(xPos,yPos);
			
			tween.TweenProperty(children[i], "position", position, .05f).SetEase(Tween.EaseType.Out);
			
			xPos += overlapX;


		}

		topBounds = node.Position.Y - overlapY;
		botBounds = yPos - overlapY;
		
		while (tween != null && tween.IsRunning())
			yield return null;
	

	}

	
	
	
}
