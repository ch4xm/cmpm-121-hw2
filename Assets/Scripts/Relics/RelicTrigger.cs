using System;
using UnityEngine;
using System.Collections;
using System.Dynamic;
using JetBrains.Annotations;

public abstract class RelicTrigger
{
    public string description;
    public string type;
    public string? amount;

    public RelicTrigger() {
    }

    public abstract void Create(RelicEffect effect);
}

public class TakeDamageTrigger : RelicTrigger
{
    public override void Create(RelicEffect effect)
    {
        EventBus.Instance.OnDamage += (where, dmg, target) =>
        {
        	if (target.team == Hittable.Team.PLAYER)
        		effect.Apply(target);
        };
    }
}

public class StandStillTrigger : RelicTrigger
{
    public override void Create(RelicEffect effect)
    {
        EventBus.Instance.OnStandStill += (time, target) =>
        {
            if (amount == null || time >= RPNEvaluator.RPNEvaluator.Evaluate(amount, null))
            {
                effect.Apply(target);
            }
        };
    }
}

public class MoveTrigger: RelicTrigger
{
    public override void Create(RelicEffect effect)
    {
        EventBus.Instance.OnMove += (time, target) =>
        {
            if (amount == null || time >= RPNEvaluator.RPNEvaluator.Evaluate(amount, null))
            {
                effect.Apply(target);
            }
        };
    }
}

public class KillTrigger: RelicTrigger
{
    public override void Create(RelicEffect effect)
    {
        EventBus.Instance.OnKill += (time, source, target) =>
        {
            Debug.Log("Trigger onkill");
            effect.Apply(source);
        };
    }
}