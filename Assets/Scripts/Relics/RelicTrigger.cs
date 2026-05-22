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

public class TakeDamageTrigger : RelicTrigger
{
    public override void create(RelicEffect effect)
    {
        EventBus.Instance.OnDamage += (where, dmg, target) =>
        {
        	if (target.team == Hittable.Team.PLAYER)
        		effect.effect(target);
        };
    }
}

public class StandStillTrigger : RelicTrigger
{
    public override void create(RelicEffect effect)
    {
        EventBus.Instance.OnStandStill += (time, target) =>
        {
            if (amount == null || time >= RPNEvaluator.RPNEvaluator.Evaluate(amount, null))
            {
                effect.effect(target);
            }
        };
    }
}

public class MoveTrigger: RelicTrigger
{
    public override void create(RelicEffect effect)
    {
        EventBus.Instance.OnMove += (time, target) =>
        {
            if (amount == null || time >= RPNEvaluator.RPNEvaluator.Evaluate(amount, null))
            {
                effect.effect(target);
            }
        };
    }
}