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
    public string sprite;

    [JsonConverter(typeof(TriggerConverter))]
    public RelicTrigger trigger;
    [JsonConverter(typeof(EffectConverter))]
    public RelicEffect effect;

    public RelicTrigger? endTrigger;
    public RelicEffect? endEffect;

    public void Activate()
    {
        Debug.Log("Activating Relic: "  + name);
        Debug.Log(trigger.description + " " + effect.description);
        trigger.Create(effect);
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

        RelicTrigger trigger = type switch
        {
            "take-damage" => new TakeDamageTrigger(),
            "stand-still" => new StandStillTrigger(),
            "on-kill" => new KillTrigger(),
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

        RelicEffect effect = type switch
        {
            "gain-mana" => new GainManaEffect(),
            "gain-spellpower" => new GainSpellPowerEffect(),
            _ => throw new Exception("Unknown type")
        };

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

