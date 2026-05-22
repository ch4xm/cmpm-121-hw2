using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;
using UnityEngine;
using System;
using System.Collections;
using System.Dynamic;
using JetBrains.Annotations;

public class Relic
{
    public string name;
    public string sprite;
    
    [JsonConverter(typeof(TriggerConverter))]
    public RelicTrigger trigger;
    //public RelicEffect effect;

    public RelicTrigger? endTrigger;
    public RelicEffect? endEffect;
    
    public Relic()
    {
        Debug.Log("Creating Relic: " + this.name);
    }
    
    
    
    
    /*
    private static void CreateTrigger(RelicTrigger trigger, string type)
    {
        switch (type)
        {
            case "take-damage":
                trigger = new TakeDamageTrigger();
            case "stand-still":
                return new StandStillTrigger();
            case "move":
                return new MoveTrigger();
            default:
                Debug.Log("Unknown trigger type: " + trigger.type);
                return null;
        }
    }

    private static object CreateEffect(RelicEffect effect)
    {
        switch (effect.type)
        {
            case "gain-mana":
                return new GainManaEffect();
            default:
                Debug.Log("Unknown effect type: " + effect.type);
                return null;
        }
    }
    */
}


class TriggerConverter : JsonConverter<RelicTrigger>
{
    public override RelicTrigger ReadJson(
        JsonReader reader,
        Type objectType,
        RelicTrigger existingValue,
        bool hasExistingValue,
        JsonSerializer serializer)
    {
        JObject obj = JObject.Load(reader);

        string type = obj["type"]?.ToString();

        RelicTrigger trigger = type switch
        {
            "take-damage" => new TakeDamageTrigger(),
            "stand-still" => new StandStillTrigger(),
            _ => throw new Exception("Unknown type")
        };

        serializer.Populate(obj.CreateReader(), trigger);

        return trigger;
    }
    
    public override void WriteJson(
        JsonWriter writer,
        RelicTrigger value,
        JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
}
