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

    public abstract void Create(Action<Hittable> effect);
}

public class TakeDamageTrigger : RelicTrigger
{
    public override void Create(Action<Hittable> effect)
    {
        EventBus.Instance.OnDamage += (where, dmg, target) =>
        {
        	if (target.team == Hittable.Team.PLAYER)
        		effect(target);
        };
    }
}

public class StandStillTrigger : RelicTrigger
{
    public override void Create(Action<Hittable> effect)
    {
        EventBus.Instance.OnStandStill += (time, target) =>
        {
            if (amount == null || time >= RPNEvaluator.RPNEvaluator.Evaluate(amount, null))
            {
                effect(target);
            }
        };
    }
}

public class MoveTrigger: RelicTrigger
{
    public override void Create(Action<Hittable> effect)
    {
        EventBus.Instance.OnMove += (time, target) =>
        {
            if (amount == null || time >= RPNEvaluator.RPNEvaluator.Evaluate(amount, null))
            {
                effect(target);
            }
        };
    }
}

public class KillTrigger : RelicTrigger
{
    public override void Create(Action<Hittable> effect)
    {
        EventBus.Instance.OnKill += (time, source, target) =>
        {
            Debug.Log("Trigger onkill");
            effect(source);
        };
    }
}

public class CastSpellTrigger : RelicTrigger
{
    public override void Create(Action<Hittable> effect)
    {
        EventBus.Instance.OnCastSpell += (source) =>
        {
            Debug.Log("Trigger cast spell");
            effect(source);
        };
    }
}

public class NotTakingDamageTrigger : RelicTrigger
{
    public override void Create(Action<Hittable> effect)
    {
        EventBus.Instance.OnNotTakingDamage += (time, target) =>
        {
            if (time >= RPNEvaluator.RPNEvaluator.Evaluate(amount, null) &&
                target.team == Hittable.Team.PLAYER)
            {
                effect(target);
            }
        };
    }
}