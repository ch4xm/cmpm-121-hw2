using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEditor.AdaptivePerformance.Editor;
using UnityEngine;
using static UnityEngine.UI.CanvasScaler;

public class Projectile
{
    public Spell parent;
    private int sprite;
    private string trajectory;
    private string speed;
    private string? lifetime;

    public Projectile(int sprite, string trajectory, string speed, string? lifetime)
    {
        this.sprite = sprite;
        this.trajectory = trajectory;
        this.speed = speed;
        this.lifetime = lifetime;
    }
    public float GetSpeed()
    {
        var result = parent.CalculateProperty(speed);

        return result;
    }

    public int GetSprite()
    {
        return sprite;
    }

    public float? GetLifetime()
    {
        if (lifetime is null)
        {
            return null;
        }
        var result = parent.CalculateProperty(lifetime);
        return result;
    }

    public string GetTrajectory()
    {
        return trajectory;
    }

}

public class Spell
{
    public float last_cast;
    public SpellCaster owner;
    public Hittable.Team team;

    private string name;
    private string description;
    private int icon;
    private DamageData damage;
    private string mana_cost;
    private float cooldown;

    private Projectile projectile;
    private Projectile secondary_projectile;

    public Spell(SpellCaster owner, SpellData data)
    {
        this.owner = owner;

        name = data.name;
        description = data.description;
        icon = data.icon;
        damage = data.damage;
        mana_cost = data.mana_cost;
        cooldown = data.cooldown;
        
        if (data.projectile != null)
        {
            projectile = data.projectile;
            projectile.parent = this;
        }
        if (data.secondary_projectile != null)
        {
            secondary_projectile = data.secondary_projectile;
            secondary_projectile.parent = this;
        }
    }

    public string GetName()
    {
        return name;
    }

    public string GetDescription()
    {
        return description;
    }

    public int GetManaCost()
    {
        return 10;
        var result = CalculateProperty(mana_cost);

        return Mathf.RoundToInt(result);
    }

    public float CalculateProperty(string formula)
    {
        var dict = new Dictionary<string, int>
        {
            { "power", owner.spell_power },
            { "wave", GameManager.Instance.currentWave }
        };
        float calculated = RPNEvaluator.RPNEvaluator.Evaluatef(formula, dict);
        return calculated;
    }

    public int GetDamage()
    {
        var result = CalculateProperty(damage.amount);

        return Mathf.RoundToInt(result);
    }
    
    public Damage.Type GetDamageType()
    {   
        return Damage.TypeFromString(damage.type);
    }
    public float GetCooldown()
    {
        return cooldown;
    }

    public virtual int GetIcon()
    {
        return icon;
    }
    
    public bool IsReady()
    {
        return (last_cast + GetCooldown() < Time.time);
    }

    public virtual IEnumerator Cast(Vector3 where, Vector3 target, Hittable.Team team)
    {
        this.team = team;

        GameManager.Instance.projectileManager.CreateProjectile(projectile.GetSprite(), projectile.GetTrajectory(), where, target - where, projectile.GetSpeed(), OnHit, projectile.GetLifetime());

        yield return new WaitForEndOfFrame();
    }

    void OnHit(Hittable other, Vector3 impact)
    {
        if (other.team != team)
        {
            other.Damage(new Damage(GetDamage(), GetDamageType()));
        }

    }

}
