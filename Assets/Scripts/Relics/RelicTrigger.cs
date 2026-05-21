using System;
using UnityEngine;
using System.Collections;
using System.Dynamic;

public abstract class RelicTrigger
{
    public string description;
    public string type;
}

public class TakeDamage : RelicTrigger
{
    public void create(RelicEffect effect)
    {
        EventBus.Instance.OnDamage += (where, dmg, target) =>
        {
            Debug.Log("trigger take damage");
        	if (target.team == Hittable.Team.PLAYER)
        		effect.effect(target);
        };
    }
}