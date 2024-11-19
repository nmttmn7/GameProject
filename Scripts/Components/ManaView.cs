using System.Collections;
using System.Collections.Generic;
using Godot;

using TheLiquidFire.Notifications;

public partial class ManaView : Node {
	
	
	[Export]public RichTextLabel richTextLabel;
	[Export]public ColorRect colorRect;
	Tween mana_tween;
	Tween mana_shake;
	public override void _Ready()
	{
		this.AddObserver (OnManaValueChangedNotification, ManaSystem.ValueChangedNotification);
	}

    public override void _ExitTree()
    {
		this.RemoveObserver (OnManaValueChangedNotification, ManaSystem.ValueChangedNotification);
	}

	void OnManaValueChangedNotification (object sender, object args) {
		var mana = args as Mana;
		string text = "[center]";
		text += mana.Available.ToString();
		richTextLabel.Text = text;
		
		if(mana_tween != null && mana_tween.IsRunning()){
			mana_tween.Kill();
		}
	
		float manaDifference = mana.Available / mana.Unlocked;
		
		mana_tween = CreateTween();
		mana_tween.TweenProperty(colorRect.Material, "shader_parameter/fill_value", manaDifference * 2.30, .2);

/*		//Shake
		if(mana_shake != null && mana_shake.IsRunning())
			mana_shake.Kill();

		mana_tween = CreateTween();
		mana_tween.TweenProperty(colorRect.Material, "shader_parameter/waveAmp", 0.6, .4);
		mana_tween.TweenProperty(colorRect.Material, "shader_parameter/waveAmp", -0.6, .4);
		mana_tween.TweenProperty(colorRect.Material, "shader_parameter/waveAmp", 0.05, .4);
		mana_tween.TweenProperty(colorRect.Material, "shader_parameter/waveFreq", 4, .4);
			mana_tween.TweenProperty(colorRect.Material, "shader_parameter/waveFreq", 2, .4); */
	
	}

	
}