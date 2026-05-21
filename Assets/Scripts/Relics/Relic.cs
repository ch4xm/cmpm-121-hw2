using UnityEngine;
using System.Collections;
using System.Dynamic;

public class Relic
{
    public string name;
    public string description;
    public string sprite;
    
    public Relic(string name, string description, string sprite)
    {
        Debug.Log("Creating Relic");
        
        this.name = name;
        this.description = description;
        this.sprite = sprite;

        var relicTrigger = new TakeDamage();
        var relicEffect = new GainMana();
        relicEffect.amount = "20";
        
        relicTrigger.create(relicEffect);
    }
}
