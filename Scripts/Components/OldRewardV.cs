using System.Collections;
using System.Collections.Generic;

using TheLiquidFire.Notifications;
using TheLiquidFire.AspectContainer;
using Godot;
using System.Numerics;
using System.Linq;
using TheLiquidFire.Extensions;


public partial class OldRewardView : Node, IAspect
{
    /*
        // Called when the node enters the scene tree for the first time.

        public IContainer container { 
            get {
                if (_container == null) {
                    _container = new TheLiquidFire.AspectContainer.Container();
                    _container.AddAspect (this);
                    _container.AddAspect<ActionSystem> ();
                }
                return _container;
            }
            set {
                _container = value;
            }
        }
        IContainer _container;

        ActionSystem actionSystem;


        [Export] PackedScene cardConstruct;
        [Export] Node2D rewardNode2D;
        [Export] DisplayObjectsView displayObjectsView;

            public override void _EnterTree()
        {	
            container.Awake ();
            actionSystem = container.GetAspect<ActionSystem> ();
            this.AddObserver (OnValidateReward, Global.ValidateNotification<RewardAction> ());
            var action = new RewardAction ();
            container.Perform(action);
            }

            public override void _ExitTree()
            {	
            this.RemoveObserver (OnValidateReward, Global.ValidateNotification<RewardAction> ());
            }

            public override void _Process(double delta)
        {
            actionSystem.Update ();
        }

            void OnValidateReward (object sender, object args) {
            var action = sender as RewardAction;
            action.perform.viewer = OnPerformRewardView;
        }

        int elementIndent = 3;
        IEnumerator OnPerformRewardView
         (IContainer game, GameAction action) {
            GenerateRewards();

            var children = rewardNode2D.GetChildren();



            if(children.Count < elementIndent)
                elementIndent = children.Count;

            var overlapY = 200f;
            var overlapX = 250f;
            var width = elementIndent * overlapX;
            var xPos = -(width / 2f);
            var yPos = 0f;

            xPos += overlapX / 2;

            //var duration = animated ? 0.25f : 0;
            Tween tween = CreateTween();
            for (int i = 0; i < children.Count; ++i)
            {


            if(i % elementIndent == 0 && i > 0){
             yPos -= overlapY;
             xPos = -(width / 2f);
             xPos += overlapX / 2;
            }

                var position = new Godot.Vector2(xPos,0);

                tween.TweenProperty(children[i], "position", position, 0.00f).SetEase(Tween.EaseType.Out);


                xPos += overlapX;


            }



        //	topBounds = node.Position.Y - overlapY;
        //	botBounds = yPos - overlapY;



            while (tween != null && tween.IsRunning())
                yield return null;

            rewardNode2D.Visible = true;



            for (int i = 0; i < children.Count; ++i)
            {


                children[i].GetChild<CardView>(0).button.Call("callInstantiate");

            }


        }
            public void GenerateRewards(){



            LoadBaseRarityChances();
            LoadLootPackChances();

            LinkedAllCardsToLootGrouping();

            if(FileFactory.IsEmpty(loadedLootFilePath)){


            var editfile =  Godot.FileAccess.Open(loadedLootFilePath,Godot.FileAccess.ModeFlags.Write);
                editfile.StoreString("{ \"deck\": [");
               for(int i = 0; i < 3; i++){

                    var baseRarity = PickAnObject("BaseRarity");
                    var lootPacks = PickAnObject("LootPacks");

                    var id = ChooseACard(baseRarity,lootPacks);
                    var card = DeckFactory.CreateCard(id, 0, null);

                    editfile.StoreString("\"" + id + "\"");
                    var instance = cardConstruct.Instantiate();

                    rewardNode2D.AddChild(instance);

                    instance.GetChild<CardView>(0).card = card;
                    instance.GetChild<CardView>(0).UpdateText();



               }

                editfile.StoreString("\n ]}");
                    editfile.Close();
            }else{

                var deck = DeckFactory.CreateDeck(loadedLootFilePath, 0);
                foreach(var card in deck){
                var instance = cardConstruct.Instantiate();
                    rewardNode2D.AddChild(instance);
                    instance.GetChild<CardView>(0).card = card;
                    instance.GetChild<CardView>(0).UpdateText();
                }

            }



            }

            public void ResetLootTable(string filePath){
                FileFactory.ClearFile(filePath);
            }

            public void AddRarityToFile(string filePath, string nameString, float val){

                if(nameString == null)
                    return;

                var file = Godot.FileAccess.Open(filePath, Godot.FileAccess.ModeFlags.Read);
                var fileText = file.GetAsText();
                file.Close();

                var temp = nameString;
                while(temp.Contains(".txt")){
            var entryPos = 0;
            var endPos = temp.IndexOf(".txt") + 4;
            var data = temp.Substring(entryPos,endPos);
            temp = temp.Remove(entryPos,endPos);

            var i = fileText.Find(data);

            if(i >= 0){
                var endText = i + data.Length + 1; 
                var rightSide = fileText.Right(endText);
                var integer = rightSide.Substring(0,rightSide.IndexOf("|"));
                float originValue = System.Convert.ToInt32(integer);
                originValue += val;
                fileText = fileText.Replace(data + ":" + integer,data + ":" + originValue);

            }else{
                fileText += "|" + data + ":" + val + "|";
            }

                }
                var editfile =  Godot.FileAccess.Open(filePath,Godot.FileAccess.ModeFlags.Write);
                editfile.StoreString(fileText);

                editfile.Close();

            }


        private float lootPackIncrement = 1;

        public void SelectCard(Card card){

            DeckFactory.Cards[card.id].TryGetValue("lootPacks", out var lootPack);

            AddRarityToFile(lootPacksWeightFilePath,lootPack.ToString(),lootPackIncrement);
            AddRarityToFile(rarityWeightFilePath, "Common.txt", 25);

        }



        private void LinkedAllCardsToLootGrouping()
        {

        var dir = DirAccess.Open("res://UserData/Cards/Player/DeckPacks/LootPacks/");
        dir.ListDirBegin();
        var allFiles = dir.GetFiles();
        dir.ListDirEnd();

        foreach (var file in allFiles){

            var deckFile = Godot.FileAccess.Open("res://UserData/Cards/Player/DeckPacks/LootPacks/" + file, Godot.FileAccess.ModeFlags.Read);
            var fileText = deckFile.GetAsText();

            var contents = MiniJSON.Json.Deserialize (fileText) as Dictionary<string, object>;
            deckFile.Close();


            var array = (List<object>)contents ["deck"];

            foreach (object item in array) {
                var id = (string)item;
                var cardData = DeckFactory.Cards[id];
                if(cardData.TryGetValue("lootPacks", out var lootPack)){

                    if(!lootPack.ToString().Contains(file)){
                    cardData["lootPacks"] += file;
                    }

                }else{
                    cardData.Add("lootPacks", file);
                }
            }

        }

        }



        public string ChooseACard(chanceObject BaseRarity, chanceObject LootPacks)
        {


            var file = Godot.FileAccess.Open(LootPacks.chance_name, Godot.FileAccess.ModeFlags.Read);
            var fileText = file.GetAsText();
            var contents = MiniJSON.Json.Deserialize (fileText) as Dictionary<string, object>;
            file.Close();


            var array = (List<object>)contents ["deck"];
            var result = new List<string> ();
            foreach (object item in array) {
                var id = (string)item;
                var cardData = DeckFactory.Cards[id];
                var rarity = (string)cardData["rarity"];

                if(rarity == BaseRarity.chance_name){
                    result.Add(id);
                }

            }

            if(result.Count == 0){
                GD.Print("|ERROR: " + LootPacks.chance_name + " DOES NOT HAVE ANY " + BaseRarity.chance_name + " cards|");
                GD.PushError(LootPacks.chance_name + " DOES NOT HAVE ANY " + BaseRarity.chance_name + " cards");
                var rng = RNGFactory.RandiRange(0, array.Count - 1);
                return (string)array[rng];

            }

            var cardID = result.Draw();

            return cardID;


        }	


        private string baseRarityFilePath = "res://UserData/LootTable/BaseRarity.txt";
        public static string rarityWeightFilePath = "res://UserData/LootTable/RarityWeight.txt";
        public static string lootPacksWeightFilePath = "res://UserData/LootTable/LootPacksWeight.txt";
        public static string loadedLootFilePath = "res://UserData/LootTable/LoadedLoot.txt";
        public void LoadBaseRarityChances(){


                var lootRarityText = Godot.FileAccess.Open(rarityWeightFilePath, Godot.FileAccess.ModeFlags.Read);
                var lootRarityTextString = lootRarityText.GetAsText();
                lootRarityText.Close();

                var file = Godot.FileAccess.Open(baseRarityFilePath, Godot.FileAccess.ModeFlags.Read);
                var fileText = file.GetAsText();
                var dict = MiniJSON.Json.Deserialize (fileText) as Dictionary<string, object>;
                file.Close();


                var array = (List<object>)dict ["loot"];

                List<chanceObject> lootBase = new();
                foreach (object entry in array) {
                    chanceObject chanceObject = new();
                    var data = (Dictionary<string, object>)entry;

                    chanceObject.chance_name = (string)data["type"];
                    chanceObject.chance_weight = System.Convert.ToInt32(data["roll_weight"]);

                    var i = lootRarityTextString.Find(chanceObject.chance_name + ".txt");

            if(i >= 0){

                var endText = i + chanceObject.chance_name.Length + 1 + ".txt".Length; 
                var rightSide = lootRarityTextString.Right(endText);
                var integer = rightSide.Substring(0,rightSide.IndexOf("|"));
                float originValue = System.Convert.ToInt32(integer);
                chanceObject.chance_weight += originValue;

            }
                    lootBase.Add(chanceObject);
                }


                InitializeProbabilities("BaseRarity", lootBase);
        }
        public void LoadLootPackChances(){


        var lootPackText = Godot.FileAccess.Open(lootPacksWeightFilePath, Godot.FileAccess.ModeFlags.Read);
        var fileText = lootPackText.GetAsText();
        lootPackText.Close();

        var dir = DirAccess.Open("res://UserData/Cards/Player/DeckPacks/LootPacks/");
        dir.ListDirBegin();
        var allFiles = dir.GetFiles();
        dir.ListDirEnd();
        List<chanceObject> lootPacks = new();
        foreach (var file in allFiles){
            chanceObject chanceObject = new();
            chanceObject.chance_name = "res://UserData/Cards/Player/DeckPacks/LootPacks/" + file;
            chanceObject.chance_weight = 1;

            var i = fileText.Find(file);

            if(i >= 0){
                var endText = i + file.Length + 1; 
                var rightSide = fileText.Right(endText);
                var integer = rightSide.Substring(0,rightSide.IndexOf("|"));
                float originValue = System.Convert.ToInt32(integer);
                chanceObject.chance_weight += originValue;

            }
            lootPacks.Add(chanceObject);

        }
            InitializeProbabilities("LootPacks",lootPacks);

        }
        public Dictionary<string, lootValue> weightedLists = new();
        //STATIC?????
        public void InitializeProbabilities(string key, List <chanceObject> list){
            float total_weight = 0;

            foreach(var obj in list){
                total_weight += obj.chance_weight;
                obj.acc_weight = total_weight;

            }

            lootValue lootValue = new();
            lootValue.chanceObjects = list;
            lootValue.total_weight = total_weight;


            weightedLists.Add(key, lootValue);
        }

        public chanceObject PickAnObject(string path){
                weightedLists.TryGetValue(path, out var lootValue);

                var rollObject = RNGFactory.RandfRange(0, lootValue.total_weight);

                foreach(var obj in lootValue.chanceObjects){

            if (obj.acc_weight > rollObject)
                return obj;
            }
            return null;






        }


     */
    public IContainer container { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

} 

/*
public class lootValue{
	public List<chanceObject> chanceObjects= new List<chanceObject>();
	public float total_weight;

}
public class chanceObject
{	
	public string chance_name;
	public float chance_weight;
	public float acc_weight;
} */