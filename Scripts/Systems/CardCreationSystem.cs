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
    var cardID = action.cardID;

    foreach (Card target in action.targets) {
      
    DeckFactory.TransformCard(target, cardID);
    
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
            statussystem.CreateCard(createdCard,contents,action.attachedAbility.card);
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
            statussystem.CreateCard(createdCard,contents,action.attachedAbility.card);
            player[Zones.Deck].Add(createdCard);

          }
          else{
		
            createdCard = DeckFactory.CreateCard(cardID, player.index, null);
             statussystem.InitializeCard(createdCard, DeckFactory.Cards[createdCard.id]);
            createdCard.AddAspect<Temp>();
            statussystem.ShareAfflictions(action.attachedAbility.card,createdCard);
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
