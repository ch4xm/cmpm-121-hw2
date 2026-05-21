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
        player.parent.GetComponent<PlayerController>().spellcaster.mana +=
            RPNEvaluator.RPNEvaluator.Evaluate(amount, null);
        Debug.Log("Added mana");
    }
}
