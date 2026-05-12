using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public class Projectile
{
    public int which;
    public string trajectory;
    public Vector3 where;
    public Vector3 direction;
    public float speed;
    public Action<Hittable, Vector3> onHit;
    public float? lifetime;

    public void create()
    {
        if (lifetime.HasValue) 
            GameManager.Instance.projectileManager.CreateProjectile(which, trajectory, where, direction, speed, onHit, lifetime.Value);
        else 
            GameManager.Instance.projectileManager.CreateProjectile(which, trajectory, where, direction, speed, onHit);
    }
}

public class Spell 
{
    public float last_cast;
    public SpellCaster owner;
    public string name;
    public string description;
    public int icon;
    public Damage damage;
    public int mana_cost;
    public float cooldown;
    public Projectile projectile;
    public Hittable.Team team;

    public Spell(SpellCaster owner)
    {
        this.owner = owner;
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
        return damage.amount;
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
        projectile.create();
        yield return new WaitForEndOfFrame();
    }

    void OnHit(Hittable other, Vector3 impact)
    {
        if (other.team != team)
        {
            other.Damage(new Damage(GetDamage(), damage.type));
        }

    }

}
