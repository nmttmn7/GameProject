using Godot;
using System;

public partial class StatusUI : Node
{
	CardView cardView;
	public string text;
	[Export]public Label label;
	[Export]public TextureRect statusIcon;
	[Export]public TextureRect decreaseIcon;
	public override void _Ready()
	{
		cardView = GetParent().GetParent().GetParent().GetParent().GetChild<CardView>(0);
	}
	
}
