using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;
using UnityEngine;
using System;
using System.Collections;
using System.Dynamic;
using JetBrains.Annotations;
using UnityEngine.WSA;

public class Relic
{
    public string name;
    public int sprite;
    public bool isActive = false;

    [JsonConverter(typeof(TriggerConverter))]
    public RelicTrigger trigger;
    [JsonConverter(typeof(EffectConverter))]
    public RelicEffect effect;

    public RelicTrigger? endTrigger;

    public void Activate()
    {
        Debug.Log("Activating Relic: "  + name);
        Debug.Log(trigger.description + " " + effect.description);
        isActive = true;
        trigger.Create(effect.Apply);

        if (effect.until != null)
        {
            effect.maxEffectCount = 1;
            endTrigger = stringToTrigger(effect.until);
            endTrigger.Create(effect.Remove);
        }
    }

    public bool IsActive()
    {
        return isActive;
    }
    
    public static RelicTrigger stringToTrigger(string type)
    {
        RelicTrigger trigger = type switch
        {
            "take-damage" => new TakeDamageTrigger(),
            "stand-still" => new StandStillTrigger(),
            "on-kill" => new KillTrigger(),
            "move"  => new MoveTrigger(),
            "cast-spell" => new CastSpellTrigger(),
            _ => throw new Exception("Unknown type")
        };
        
        return trigger;
    }

    public static RelicEffect stringToEffect(string type)
    {
        RelicEffect effect = type switch
        {
            "gain-mana" => new GainManaEffect(),
            "gain-spellpower" => new GainSpellPowerEffect(),
            "gain-movement-speed" => new GainMovementSpeedEffect(),
            _ => throw new Exception("Unknown type")
        };
        return effect;
    }
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
        /*
        RelicTrigger trigger = type switch
        {
            "take-damage" => new TakeDamageTrigger(),
            "stand-still" => new StandStillTrigger(),
            "on-kill" => new KillTrigger(),
            _ => throw new Exception("Unknown type")
        };
        */
        RelicTrigger trigger = Relic.stringToTrigger(type);
            
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

class EffectConverter : JsonConverter<RelicEffect>
{
    public override RelicEffect ReadJson(
        JsonReader reader,
        Type objectType,
        RelicEffect existingValue,
        bool hasExistingValue,
        JsonSerializer serializer)
    {
        JObject obj = JObject.Load(reader);

        string type = obj["type"]?.ToString();

        /*
        RelicEffect effect = type switch
        {
            "gain-mana" => new GainManaEffect(),
            "gain-spellpower" => new GainSpellPowerEffect(),
            _ => throw new Exception("Unknown type")
        };
        */
        
        RelicEffect effect = Relic.stringToEffect(type);
        
        serializer.Populate(obj.CreateReader(), effect);

        return effect;
    }
    
    public override void WriteJson(
        JsonWriter writer,
        RelicEffect value,
        JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
}

