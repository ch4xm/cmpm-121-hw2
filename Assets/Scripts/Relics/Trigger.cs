using UnityEngine;
using System.Collections;
using System.Dynamic;

public abstract class Trigger
{
    public string description;
    
    public Action event;
}

public class takeDamage : Trigger
{
    public void tigger(Vector3 where, Damage dmg, Hittable target)
    {
        EventBus.Instance.DoDamage(where, dmg, target) += event; 
    }
}