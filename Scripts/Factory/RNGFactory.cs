using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public static class RNGFactory{
    static RandomNumberGenerator rng = new();

	public static string seedPath = "res://UserData/Seed.txt";
	public static void SetSeed(string seed){
		rng.Seed = (ulong)GD.Hash(seed);
	}
	public static T RandListInteger<T>(List<T> list){
		var index = rng.RandiRange(0, list.Count - 1);
		return list[index];
	}

		public static T Random<T> (this T[] array)
		{
			
			int index = rng.RandiRange(0, array.Length - 1);
			return array[index];
		}

	public static int RandiRange(int begin, int end){
		var result = rng.RandiRange(begin,end);
		return result;
	}
	public static float RandfRange(float begin, float end){
		var result = rng.RandfRange(begin,end);
		return result;
	}
	public static void RandomSeed(){
		rng.Randomize();
	}
	public static ulong GetSeed(){
		return rng.Seed;
	}
	public static void LoadSeed(){
		
		if(!FileFactory.Contains(seedPath,"seed")){
			GD.PushError("RNGFactory File is EMPTY");
			return;
		}

		var file =  Godot.FileAccess.Open(seedPath,Godot.FileAccess.ModeFlags.Read);
		

		
		var fileText = file.GetAsText();
		var contents = MiniJSON.Json.Deserialize (fileText) as Dictionary<string, object>;
		file.Close();
		var array = (List<object>)contents ["seed"];
		var nodeData = (Dictionary<string, object>)array.First();
		ulong seed = System.Convert.ToUInt64(nodeData["seed"]);
		rng.Seed = seed;
		ulong state = System.Convert.ToUInt64(nodeData["state"]);
		rng.State = state;

	}
	public static void SaveSeed(){
		var file =  Godot.FileAccess.Open(seedPath,Godot.FileAccess.ModeFlags.Write);
		file.StoreString("{ \"seed\": [");
		file.StoreString("\n{");
		file.StoreString("\n" + "\"seed\": " + "\"" + rng.Seed + "\"");
		file.StoreString("\n" + "\"state\": " + "\"" + rng.State + "\"");
		file.StoreString("\n}");
		file.StoreString("\n ]}");
		file.Close();
	}

	public static void ClearSeedFile(){
		
		var file =  Godot.FileAccess.Open(seedPath,Godot.FileAccess.ModeFlags.Write);
		var text = file.GetAsText();
		text = "";
		file.Close();

	}
}
