using UnityEngine;
using System.Collections;
using System.Dynamic;
using JetBrains.Annotations;

public class Relic
{
    public string name;
    public string sprite;
    public RelicTrigger trigger;
    public RelicEffect effect;

    public RelicTrigger? endTrigger;
    public RelicEffect? endEffect;
    
    public Relic()
    {
        Debug.Log("Creating Relic: " + this.name);
        
        switch (trigger.type)
        {
            case "take-damage":
                trigger = new TakeDamageTrigger();
                break;
            case "stand-still":
                trigger = new StandStillTrigger();
                break;
            case "move":
                trigger = new MoveTrigger();
                break;
            default:
                Debug.Log("Unknown trigger type: " + trigger.type);
                break;
        }

        switch (effect.type)
        {
            case "gain-mana":
                effect = new GainManaEffect();
                break;
            default:
                Debug.Log("Unknown effect type: " + effect.type);
                break;
        }
        /*
        trigger = new StandStillTrigger();
        effect = new GainManaEffect();
        trigger.amount = "1";
        effect.amount = "20";
        
        trigger.create(effect);
        */
    }
}


