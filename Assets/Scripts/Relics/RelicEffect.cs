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

    public int currentEffectCount = 0;
    public int maxEffectCount = -1;

    public void Apply(Hittable player)
    {
        if (maxEffectCount >= 0 && currentEffectCount >= maxEffectCount) return;
        currentEffectCount++;

        ApplyEffect(player);
    }
    
    public abstract void ApplyEffect(Hittable player);

    public void Remove(Hittable player)
    {
        if (currentEffectCount <= 0) return;
        currentEffectCount--;
        
        RemoveEffect(player);
    }

    public virtual void RemoveEffect(Hittable player)
    {
        throw new NotImplementedException();
    }

    public void setMaxStack(int count)
    {
        maxEffectCount = count;
    }
}

public class GainManaEffect : RelicEffect
{
    public override void ApplyEffect(Hittable player)
    {
        var spellcaster = player.parent.GetComponent<PlayerController>().spellcaster;
        spellcaster.mana += (int)Relic.evaluate(amount);
        spellcaster.mana =  Mathf.Clamp(spellcaster.mana, 0, spellcaster.max_mana);
        Debug.Log("Added mana");
    }
}

public class GainSpellPowerEffect : RelicEffect
{
    public override void ApplyEffect(Hittable player)
    {
        var spellcaster = player.parent.GetComponent<PlayerController>().spellcaster;
        spellcaster.spell_power += (int)Relic.evaluate(amount);
        Debug.Log("Added spell power");
    }

    public override void RemoveEffect(Hittable player)
    {
        var spellcaster = player.parent.GetComponent<PlayerController>().spellcaster;
        spellcaster.spell_power -= (int)Relic.evaluate(amount);
        Debug.Log("Removed spell power");
    }
}

public class GainMovementSpeedEffect : RelicEffect
{
    public override void ApplyEffect(Hittable player)
    {
        player.parent.GetComponent<PlayerController>().speed += (int)Relic.evaluate(amount);
        Debug.Log("Added speed");
    }

    public override void RemoveEffect(Hittable player)
    {
        player.parent.GetComponent<PlayerController>().speed -= (int)Relic.evaluate(amount);
        Debug.Log("Removed speed");
    }
}

public class HealingOverTimeEffect : RelicEffect
{
    public override void ApplyEffect(Hittable player)
    {
        player.parent.GetComponent<PlayerController>().healingOverTime += Relic.evaluate(amount);
        Debug.Log("Added healing over time");
    }

    public override void RemoveEffect(Hittable player)
    {
        player.parent.GetComponent<PlayerController>().healingOverTime -= Relic.evaluate(amount);
        Debug.Log("Removed healing over time");
    }
}

public class HealEffect : RelicEffect
{
    public override void ApplyEffect(Hittable player)
    {
        player.parent.GetComponent<PlayerController>().hp.Heal((int)Relic.evaluate(amount));
        Debug.Log("Added healing");
    }
}

