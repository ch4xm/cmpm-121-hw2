using System;
using UnityEngine;
using System.Collections;
using System.Dynamic;
using JetBrains.Annotations;

public abstract class RelicTrigger
{
    public string description;
    public string type;
    [CanBeNull] public string amount;
    
    public abstract void create(RelicEffect effect);
}

public class TakeDamage : RelicTrigger
{
    public override void create(RelicEffect effect)
    {
        EventBus.Instance.OnDamage += (where, dmg, target) =>
        {
            Debug.Log("trigger take damage");
        	if (target.team == Hittable.Team.PLAYER)
        		effect.effect(target);
        };
    }
}

public class StandStill : RelicTrigger
{
    public override void create(RelicEffect effect)
    {
        EventBus.Instance.OnStandStill += (time, target) =>
        {
            if (time >= RPNEvaluator.RPNEvaluator.Evaluate(amount, null))
            {
                Debug.Log("trigger stand still");
                effect.effect(target);
            }
        };
    }
}