using System;
using UnityEngine;
using System.Collections;
using System.Dynamic;
using JetBrains.Annotations;
using Unity.VisualScripting;

public abstract class RelicEffect
{
    public string description;
    public string type;
    public string? amount;
    public string? until;
    
    public abstract void Apply(Hittable player);
}

public class GainManaEffect : RelicEffect
{
    public override void Apply(Hittable player)
    {
        var spellcaster = player.parent.GetComponent<PlayerController>().spellcaster;
        spellcaster.mana += RPNEvaluator.RPNEvaluator.Evaluate(amount, null);
        spellcaster.mana =  Mathf.Clamp(spellcaster.mana, 0, spellcaster.max_mana);
        Debug.Log("Added mana");
    }
}
