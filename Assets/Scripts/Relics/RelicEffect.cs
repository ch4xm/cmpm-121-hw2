using System;
using UnityEngine;
using System.Collections;
using System.Dynamic;
using JetBrains.Annotations;
using Unity.VisualScripting;

public abstract class RelicEffect
{
    public string description;
    [CanBeNull] public string amount;
    [CanBeNull] public string until;
    
    public abstract void effect(Hittable player);
}

public class GainMana : RelicEffect
{
    public override void effect(Hittable player)
    {
        var spellcaster = player.parent.GetComponent<PlayerController>().spellcaster;
        spellcaster.mana += RPNEvaluator.RPNEvaluator.Evaluate(amount, null);
        spellcaster.mana =  Mathf.Clamp(spellcaster.mana, 0, spellcaster.max_mana);
        Debug.Log("Added mana");
    }
}
