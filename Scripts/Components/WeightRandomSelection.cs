using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class WeightRandomSelection
{
	

	private static List<Item> InitializeItemWeight(List <Item> list){
		float total_weight = 0;
		 
		foreach(var item in list){
			total_weight += item.roll_weight;
			item.acc_weight = total_weight;

		}

		return list;
	}

	public static void SaveToWeightedCollection(string key, List <Item> list){
		

		if(weightedCollection.TryGetValue(key, out var weightList)){
				foreach(var item in list)
					weightList.Add(item);
		}else{

			weightedCollection.Add(key, list);
		}

	}

	public static void CalculateWeightedList(string key){
			if(weightedCollection.TryGetValue(key, out var weightList)){
				InitializeItemWeight(weightList);
			}
	}
	
	public static Item PickAWeightedItem(string path){

		weightedCollection.TryGetValue(path, out var itemList);

			var rng = RNGFactory.RandfRange(0, itemList[itemList.Count - 1].acc_weight);

			foreach(var item in itemList){
		
			if (item.acc_weight > rng)
         		return item;

			}



			
			return null;
	}

	public static List<Item> GetItemList(string path){
		if(weightedCollection.TryGetValue(path, out var itemList)){
			return itemList;
		}
		

		return null;
	}

	public static void ClearWeightCollection(){
			weightedCollection.Clear();
	}

	public static void RemoveFromWeightCollection(string key){
			weightedCollection.Remove(key);
	}

private static Dictionary<string, List<Item>> weightedCollection = new();

}
public class Item
{	
	public string name;
	public float roll_weight;
	public float acc_weight;

	public Item(string name, float roll_weight)
	{
		this.name = name;
		this.roll_weight = roll_weight;
	}
}
