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
        Player player;

        string cardID = action.cardID;

        Card createdCard = null;
        
        foreach (Card target in action.targets) {
            
         
        player = match.players[target.ownerIndex];
          var statussystem = container.GetAspect<StatusSystem>();
          if(cardID == "target"){

            var file = Godot.FileAccess.Open(DataManager.cardCreationFilePath, Godot.FileAccess.ModeFlags.Write);
            SaveFactory.SaveCard(target,file,false);
            file.Close();
            var loadfile = Godot.FileAccess.Open(DataManager.cardCreationFilePath, Godot.FileAccess.ModeFlags.Read);
		
		    var fileText = loadfile.GetAsText();
           
		    var contents = MiniJSON.Json.Deserialize (fileText) as Dictionary<string, object>;
		    loadfile.Close();

            createdCard = DeckFactory.CreateCard(null, player.index, contents);
            statussystem.MM(createdCard,contents);
            createdCard.AddAspect<Temp>();
            player[Zones.Deck].Add(createdCard);
            
          }else if(cardID == "self"){

            var file = Godot.FileAccess.Open(DataManager.cardCreationFilePath, Godot.FileAccess.ModeFlags.Write);
            SaveFactory.SaveCard(action.attachedAbility.card,file,false);
            file.Close();
            var loadfile = Godot.FileAccess.Open(DataManager.cardCreationFilePath, Godot.FileAccess.ModeFlags.Read);
		
		    var fileText = loadfile.GetAsText();
            
		    var contents = MiniJSON.Json.Deserialize (fileText) as Dictionary<string, object>;
		    loadfile.Close();

            createdCard = DeckFactory.CreateCard(null, player.index, contents);
            createdCard.AddAspect<Temp>();
            statussystem.MM(createdCard,contents);
            player[Zones.Deck].Add(createdCard);

          }
          else{
		
            createdCard = DeckFactory.CreateCard(cardID, player.index, null);
            createdCard.AddAspect<Temp>();
            player[Zones.Deck].Add(createdCard);
        
          }

        
        
        if(action.drawCreatedCard == true)
            DrawCreatedCard(player, createdCard);
       
        }

	}
  
    private void DrawCreatedCard(Player player, Card createdCard)
    {
        var newAction = new DrawCardsAction(player.GetACard(), 1);
        newAction.cards.Add(createdCard);
	      newAction.createdCard = true;
		    container.AddReaction(newAction);
    }

}
