using System.Collections;
using System.Collections.Generic;

using TheLiquidFire.AspectContainer;
using System;
using Godot;
using TheLiquidFire.Extensions;

public class TransformCardsAction : GameAction, IAbilityLoader
{
	public List<Card> targets;
	public Ability attachedAbility;

	public string cardID;
	#region Constructors
	public TransformCardsAction()
	{

	}

	public TransformCardsAction(Card target)
	{
		targets = new List<Card>(1);
		targets.Add(target);

	}

	public TransformCardsAction(List<Card> targets)
	{
		this.targets = targets;

	}
	#endregion

	#region IAbility
	public void Load(IContainer game, Ability ability)
	{
		attachedAbility = ability;
		var targetSelector = ability.GetAspect<ITargetSelector>();
		var cards = targetSelector.SelectTargets(game);
		targets = new List<Card>();
		foreach (Card card in cards)
		{
			if (card != null)
				targets.Add(card);
		}

		cardID = ability.userInfo.ToString();


	}

	#endregion





	public string LoadText(Ability ability)
	{
		string text = "";

		
		return text += "\n" + "\n";
		/*	
			string text = "";
			int info = ConvertUserInfo(ability,player);
			Status status = ability.GetAspect<Status>();

			if(info > 0){
			text += "Deal ";
			text +=  info + " ";
			text += "damage. ";
			}
			if(status != null){
				string statStr = status.increase.ToString();
				text += "Apply ";
				if(status.modifier.Contains("x")){
					text += "x ";
				}
				if(status.increase.ToString().Contains("info")){
					text += "damage value ";
				}

				if(statStr.Contains("card")){
					statStr = statStr.Replace("card", "");
					text += statStr + " ";

				}else if(statStr.Contains("target")){
					statStr = statStr.Replace("target", "");
					text += "Target ";
					text += statStr + " ";
				}else
				text += status.increase + " ";

				text += "[img=40]" + status.sprite + "[/img]. ";
			}

			return text += "\n" + "\n";
		//	return text; */
	}



}