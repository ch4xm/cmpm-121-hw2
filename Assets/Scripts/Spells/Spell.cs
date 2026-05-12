using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.CanvasScaler;

public class Projectile
{
    public int sprite;
    public string trajectory;
    public float speed;
    public float? lifetime;

    public Projectile(ProjectileData data)
    {
        sprite = data.sprite;
        trajectory = data.trajectory;
        speed = data.speed;
        lifetime = data.lifetime;
    }
}

public class Spell
{
    public float last_cast;
    public SpellCaster owner;
    public Hittable.Team team;

    public string name;
    public string description;
    public int icon;
    public DamageData damage;
    public int mana_cost;
    public float cooldown;
    public ProjectileData projectile;
    public ProjectileData secondary_projectile;

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

        GameManager.Instance.projectileManager.CreateProjectile(projectile.sprite, projectile.trajectory, where, target - where, projectile.speed, OnHit, projectile.lifetime);

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
