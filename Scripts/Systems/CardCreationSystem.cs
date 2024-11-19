using System;
using System.Collections;
using System.Collections.Generic;
using Godot;

using TheLiquidFire.AspectContainer;
using TheLiquidFire.Extensions;
using TheLiquidFire.Notifications;
public partial class CardCreationSystem : Aspect, IObserve
{

    public void Awake()
    {
       this.AddObserver(OnPerformCreateCardAction, Global.PerformNotification<CreateCardsAction>(), container);
       this.AddObserver(OnPerformTransformCardsAction, Global.PerformNotification<TransformCardsAction>(), container);
    }

    public void Destroy()
    {
     this.RemoveObserver(OnPerformCreateCardAction, Global.PerformNotification<CreateCardsAction>(), container);
     this.RemoveObserver(OnPerformTransformCardsAction, Global.PerformNotification<TransformCardsAction>(), container);
    
    }
	
    void OnPerformTransformCardsAction(object sender, object args)
	{
        var action = args as TransformCardsAction;
        


        Card createdCard;

        foreach (var card in action.targets){

		createdCard = DeckFactory.CreateCard(action.cardID,card.ownerIndex, null);
        
        var newAbilityChain = createdCard.GetAspect<AbilityRoot>().abilityChain;
        
        foreach (var ability in newAbilityChain){
            ability.card = card;
            ability.container = card;
        }

        card.GetAspect<AbilityRoot>().abilityChain = newAbilityChain;
        card.cost = createdCard.cost;
        card.description = createdCard.description;
        card.spritePath = createdCard.spritePath; 
        
        
        }
        
    }
	
	void OnPerformCreateCardAction(object sender, object args)
	{
        var action = args as CreateCardsAction;
        var match = container.GetAspect<DataSystem>().match;
        Player player = match.CurrentPlayer;

        string cardID = action.cardID;

        
        
        foreach (Card target in action.targets) {
            if(action.targetInfo.Equals("target"))
                player = match.players[target.ownerIndex];
            if(action.targetInfo.Equals("self"))
                player = match.CurrentPlayer;
            if(action.targetInfo.Equals("oppose"))
                player = match.OpponentPlayer;
                
    
        if(action.cardID.ToLower().Equals("copy"))
            cardID = target.id;
            
		var createdCard = DeckFactory.CreateCard(cardID, player.index, null);
        createdCard.AddAspect<Temp>();
        player[Zones.Deck].Add(createdCard);
    
        if(action.drawCreatedCard == true)
            DrawCreatedCard(player, createdCard);
       
        }

	}
  
    private void DrawCreatedCard(Player player, Card createdCard)
    {
         var newAction = new DrawCardsAction(player, 1);
		newAction.createdCard = true;
        newAction.findCard.Add(createdCard);
		container.AddReaction(newAction);
    }

}
