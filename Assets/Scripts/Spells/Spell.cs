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
    private int sprite;
    private string trajectory;
    private string speed;
    private float? lifetime;

    public Projectile(int sprite, string trajectory, string speed, float? lifetime)
    {
        this.sprite = sprite;
        this.trajectory = trajectory;
        this.speed = speed;
        this.lifetime = lifetime;
    }
    public float GetSpeed()
    {
        var dict = new Dictionary<string, int>
        {
            { "power", 1 },
            { "wave", GameManager.Instance.currentWave }
        };
        float calculated = RPNEvaluator.RPNEvaluator.Evaluatef(speed, dict);

        return calculated;
    }

    public int GetSprite()
    {
        return sprite; 
    }

    public float? GetLifetime()
    {
        return lifetime;
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
    private int mana_cost;
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

        projectile = data.projectile;
        secondary_projectile = data.secondary_projectile;
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
        return mana_cost;
    }

    public int GetDamage()
    {
        var dict = new Dictionary<string, int>
        {
            { "power", 1 },
            { "wave", GameManager.Instance.currentWave }
        };
        float calculated = RPNEvaluator.RPNEvaluator.Evaluatef(damage.amount, dict);
        return Mathf.RoundToInt(calculated);
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
