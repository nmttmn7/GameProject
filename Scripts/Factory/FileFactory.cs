using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

public static class FileFactory{

//public static string mapPath = "res://UserData/Map/Map.txt";
//public static string enemyDeckPath = "res://GameData/Enemy/EnemyDeck.txt";



public static int FindNumberOfStringInstances(string path, string text){
				var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
				var fileText = file.GetAsText();
				file.Close();
				var count = Regex.Matches(fileText,text).Count;
				return count;
				
}

public static bool IsEmpty(string path){
				var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
				var fileText = file.GetAsText();
				file.Close();
				if(fileText.Length <= 0){
					return true;
				}

					return false;

}

public static bool Contains(string path, string value){
				var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
				var fileText = file.GetAsText();
				file.Close();
				if(fileText.Contains(value)){
					return true;
				}

					return false;

}
public static int NumberOfFile(string path){

	var dir = DirAccess.Open(path);
	dir.ListDirBegin();
	int count = 0;
	while(true){
		
		if(dir.GetNext() == ""){
		dir.ListDirEnd();
		return count;
		}
		
	count++;

	}
	
}

public static void RemoveFile(DirAccess dir, string path){
	
	if(path == DeckFactory.cardCompendiumPath){
		GD.PushError("FILEFACTORY TRIED TO DELETE CARD COMPENDIUM");
		return;
	}

	dir.Remove(path);
}

public static void ClearFile(string path){

	if(path == DeckFactory.cardCompendiumPath){
		GD.PushError("FILEFACTORY TRIED TO DELETE CARD COMPENDIUM");
		return;
	}
	
	var file = FileAccess.Open(path, FileAccess.ModeFlags.Write);
	var fileText = file.GetAsText();
	fileText = "";
	file.Close();
			
}
}
