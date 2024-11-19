using System.Collections;
using System.Collections.Generic;
using Godot;


public partial class DeckView : Node {
	public Node2D topCard;
    //[SerializeField] Transform squisher;
    public override void _Ready()
    {
     //   topCard = GetChild<Node2D>(0);
    }
    public void ShowDeckSize (float size) {
		//squisher.localScale = Mathf.IsEqualApprox (size, 0) ? Vector3.Zero : new Vector3 (1, size, 1);
	}
}