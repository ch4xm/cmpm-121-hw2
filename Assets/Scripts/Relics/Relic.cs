using UnityEngine;
using System.Collections;
using System.Dynamic;
using JetBrains.Annotations;

public class RelicTriggerData
{
    public string discription;
    public string type;
    [CanBeNull] public string amount;
}

public class RelicEffectData
{
    public string discription;
    public string type;
    [CanBeNull] public string amount;
    [CanBeNull] public string until;
}

public class Relic
{
    public string name;
    public string sprite;
    
    public RelicTriggerData relicTriggerData;
    public RelicEffectData relicEffectData;
    
    public Relic()
    {
        Debug.Log("Creating Relic");
        
        var relicTrigger = new TakeDamage();
        var relicEffect = new GainMana();
        relicEffect.amount = "20";
        
        relicTrigger.create(relicEffect);
    }
}
