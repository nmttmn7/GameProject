using Godot;
using System;
using System.Collections.Generic;
using TheLiquidFire.AspectContainer;

public  interface IEvoke: IAspect{

    bool status {get;}
    int decreaseAmount {get;}
    int incrementAmount {get; set;}
    public object statusInfo { get; set; }
	
    public string Save(){
        string text = "";
        text += "\n\"type\": " + "\"" + GetType() + "\","; 
        text += "\n\"statusValue\": " + "\"" + statusInfo.ToString() + "\","; 
        return text;
    }
}

