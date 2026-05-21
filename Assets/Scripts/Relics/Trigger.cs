using UnityEngine;
using System.Collections;
using System.Dynamic;
using System;

public abstract class Trigger
{
    public string description;
    
    public Action Event;
}

public class TakeDamage : Trigger
{
    public void Trigger(Vector3 where, Damage dmg, Hittable target)
    {
        //EventBus.Instance.DoDamage(where, dmg, target) += Event; 
    }
}