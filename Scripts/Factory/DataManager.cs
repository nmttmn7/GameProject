using System.Collections;
using System.Collections.Generic;
using Godot;
using System;
using System.Linq;

public partial class DataManager : Node
{

	public static DataManager node;
	public override void _Ready()
	{
		node = this;

	}

	[Export] public PackedScene animationShake;
	[Export] public PackedScene cardConstruct;
	[Export] public PackedScene statusIconConstruct;
	public static string mapPath = "res://Data/UserData/Map.txt";

	public static string saveScene = "res://Data/UserData/SaveScene.txt";

	public static string seedPath = "res://Data/UserData/Seed.txt";
	
	public static string dispositionPath = "res://Data/UserData/Disposition.txt";

	public static string playerdeckPath = "res://Data/UserData/PlayerDeck.txt";
	public static string enemydeckPath = "res://Data/UserData/EnemyDeck.txt";

	public static string placeHolderDeck = "res://Data/PackCollection/Player/D1StarterPack.txt";





	
	public static string rarityWeightFilePath = "res://Data/PackCollection/Player/DeckPacks/LootTable/RarityWeight.txt";
	public static string lootGroupWeightFilePath = "res://Data/PackCollection/Player/DeckPacks/LootTable/LootGroupWeight.txt";


	public static string loadedLootFilePath = "res://Data/PackCollection/Player/DeckPacks/LootTable/LoadedLoot.txt";
	public static string baseRarityFilePath = "res://Data/PackCollection/Player/DeckPacks/LootTable/BaseRarity.txt";

	public static string cardCreationFilePath = "res://Data/UserData/CardCreationTemp.txt";
	public static string  lootPacksPath = "res://Data/UserData/LootPacks.txt";
	
}
