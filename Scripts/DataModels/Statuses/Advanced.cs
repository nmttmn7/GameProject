using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;

[Tool]
[GlobalClass]
public partial class Advanced : Node
{
   
   //https://www.youtube.com/watch?v=PrCza2z0Log
   //https://docs.godotengine.org/en/stable/classes/class_%40globalscope.html#enum-globalscope-propertyhint

String statusdataStatId;
String statusdataStatName;
Texture2D statusdataStatSprite;
bool statusdataStatValueToAbility;
StatusTypes statusdataTypesStatusType;
EvokeTypes statusdataTypesEvokeType;
int statusdataDecrementAmount = 1;
bool statusdataDecrementReset = false;

Array<AbilityData> statusdataAbilitiesAbilityChain;

    public override Godot.Collections.Array<Godot.Collections.Dictionary> _GetPropertyList()
    {
        var properties = new Godot.Collections.Array<Godot.Collections.Dictionary>();

	

			properties.Add(new Godot.Collections.Dictionary()
            {
                { "name", "StatusData" },
                { "type", (int)Variant.Type.Nil },
				{ "usage", (int)PropertyUsageFlags.Category},
				{ "hint_string", "statusdata"},
            });

			properties.Add(new Godot.Collections.Dictionary()
            {
                { "name", "Stat" },
                { "type", (int)Variant.Type.Nil },
				{ "usage", (int)PropertyUsageFlags.Group},
				{ "hint_string", "statusdataStat"},
            });

			properties.Add(new Godot.Collections.Dictionary()
            {
                { "name", "statusdataStatId" },
                { "type", (int)Variant.Type.String },
				{ "usage", (int)PropertyUsageFlags.Default }
            });

			properties.Add(new Godot.Collections.Dictionary()
            {
                { "name", "statusdataStatName" },
                { "type", (int)Variant.Type.String },
				{ "usage", (int)PropertyUsageFlags.Default }
            });

			properties.Add(new Godot.Collections.Dictionary()
            {
                { "name", $"statusdataStatSprite" },
                { "type", (int)Variant.Type.Object },
				{ "usage", (int)PropertyUsageFlags.Default},
				{ "hint", (int)PropertyHint.ResourceType},
				{ "hint_string", "Texture2D"},
            });

			properties.Add(new Godot.Collections.Dictionary()
            {
                { "name", "statusdataStatValueToAbility" },
                { "type", (int)Variant.Type.Bool },
				{ "usage", (int)PropertyUsageFlags.Default }
            });

			properties.Add(new Godot.Collections.Dictionary()
            {
                { "name", "Types" },
                { "type", (int)Variant.Type.Nil },
				{ "usage", (int)PropertyUsageFlags.Group},
				{ "hint_string", "statusdataTypes"},
            });

			properties.Add(new Godot.Collections.Dictionary()
            {
                { "name", "statusdataTypesStatusType" },
                { "type", (int)Variant.Type.Int },
				{ "usage", (int)PropertyUsageFlags.Default },
				{ "hint", (int)PropertyHint.Enum},
				{ "hint_string", String.Join(",",Enum.GetValues(typeof(StatusTypes)).Cast<StatusTypes>().ToList())},
            });
			
			properties.Add(new Godot.Collections.Dictionary()
            {
                { "name", "statusdataTypesEvokeType" },
                { "type", (int)Variant.Type.Int },
				{ "usage", (int)PropertyUsageFlags.Default },
				{ "hint", (int)PropertyHint.Flags},
				{ "hint_string", String.Join(",",Enum.GetValues(typeof(EvokeTypes)).Cast<EvokeTypes>().ToList())},
            });

			properties.Add(new Godot.Collections.Dictionary()
            {
                { "name", "Decrement" },
                { "type", (int)Variant.Type.Nil },
				{ "usage", (int)PropertyUsageFlags.Group},
				{ "hint_string", "statusdataDecrement"},
            });
			
			properties.Add(new Godot.Collections.Dictionary()
            {
                { "name", "statusdataDecrementReset" },
                { "type", (int)Variant.Type.Bool },
				{ "usage", (int)PropertyUsageFlags.Default }
            });

			properties.Add(new Godot.Collections.Dictionary()
            {
                { "name", "statusdataDecrementAmount" },
                { "type", (int)Variant.Type.Float },
				{ "usage", (int)PropertyUsageFlags.Default },
				{ "hint", (int)PropertyHint.Range},
				{ "hint_string", "-10,10,1"},
            });
			
			properties.Add(new Godot.Collections.Dictionary()
            {
                { "name", "Abilities" },
                { "type", (int)Variant.Type.Nil },
				{ "usage", (int)PropertyUsageFlags.Group},
				{ "hint_string", "statusdataAbilities"},
            });

			properties.Add(new Godot.Collections.Dictionary()
            {
                { "name", "statusdataAbilitiesAbilityChain" },
                { "type", (int)Variant.Type.Array },
				{ "usage", (int)PropertyUsageFlags.Default},
				{ "hint", (int)PropertyHint.TypeString},
				{ "hint_string", $"{Variant.Type.Object:D}/{PropertyHint.ResourceType:D}:AbilityData"
				}
				
				
            });
        

        return properties;
    }


    
}