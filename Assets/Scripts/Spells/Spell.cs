using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.AdaptivePerformance.Editor;
using UnityEngine;
using static UnityEngine.UI.CanvasScaler;

public class Projectile
{
    public Spell parent;
    public int sprite;
    public string trajectory;
    public string speed;
    public string? lifetime;

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
    private string cooldown;
    private string N;
    private string spray;
    
    private int multicast = 1;
    private int multishot = 1;
    private float angle = 0;
    private float delay;
    private bool has_secondary_projectile = false;

    

    private float stun_time = 0;
    private float last_stun_time = 0;

    private Projectile projectile;
    private Projectile secondary_projectile;

    List<ModifierData> modifiers;
    
    public Spell(SpellCaster owner, SpellData spell_data, List<ModifierData> modifiers)
    {
        this.owner = owner;
        this.modifiers = new List<ModifierData>(modifiers);

        name = spell_data.name;
        description = spell_data.description;
        icon = spell_data.icon;
        damage = spell_data.damage;
        mana_cost = spell_data.mana_cost;
        cooldown = spell_data.cooldown;
        
        N = spell_data.N ?? "1";
        spray = spell_data.spray;

        projectile = spell_data.projectile;
        projectile.parent = this;


        if (spell_data.secondary_projectile != null)
        {
            secondary_projectile = spell_data.secondary_projectile;
            secondary_projectile.parent = this;
            has_secondary_projectile = true;
        }

        if (!has_secondary_projectile) multishot *= (int)CalculateProperty(N);
            

        foreach (var modifier in modifiers)
        {
            if (modifier.time != null)
            {
                stun_time = Convert.ToSingle(modifier.time);
            }
            if (modifier.damage_multiplier != null)
            {
                damage.amount += (" " + modifier.damage_multiplier + " *");
                Debug.Log("new damage: " + damage.amount); // testing
            }
            if (modifier.mana_multiplier != null)
            {
                mana_cost += (" " + modifier.mana_multiplier + " *");
            }
            if (modifier.projectile_trajectory != null)
            {
                projectile.trajectory = modifier.projectile_trajectory;
            }
            if (modifier.speed_multiplier != null)
            {
                projectile.speed += (" " + modifier.speed_multiplier + " *");
            }
            if (modifier.cooldown_multiplier != null)
            {
                cooldown += (" " + modifier.cooldown_multiplier + " *");
            }
            if (modifier.mana_adder != null)
            {
                mana_cost += (" " + modifier.mana_adder + " +");
            }

            if (modifier.multishot != null)
            {
                multishot *= Convert.ToInt32(modifier.multishot);
            }
            if (modifier.angle != null)
            {
                angle = Convert.ToSingle(modifier.angle);
            }
            if (modifier.delay != null)
            {
                Debug.Log("multicast");
                multicast ++;
                delay = Convert.ToSingle(modifier.delay);
            }
        }
    }

    public string GetName()
    {
        return name;
    }

    public string GetFullName()
    {
        string modName = "";
        foreach (var mod in modifiers)
        {
            modName += mod.name.FirstCharacterToUpper() + " ";
        }
        return modName + name;
    }

    public string GetModifiersDescription()
    {
        string modDescription = "";
        foreach (var mod in modifiers)
        {
            modDescription += mod.name.FirstCharacterToUpper() + ": " + mod.description + "\n";
        }
        return modDescription;
    }

    public string GetDescription()
    {
        return description;
    }

    public int GetManaCost()
    {
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
        return CalculateProperty(cooldown);
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

        
        int cast_left = multicast; 
        while (cast_left > 0)
        {
            cast_left--;
            
            Vector3 dir = target - where;
            
            dir = Quaternion.Euler(0, 0, -(multishot - 1) * angle / 2) * dir;
            
            int shot_left = multishot;
            while (shot_left > 0)
            {
                shot_left--;
                if (spray != null)
                {
                    float spray_angle = UnityEngine.Random.Range(-CalculateProperty(spray) / 2, CalculateProperty(spray) / 2);
                    dir = Quaternion.Euler(0, 0, spray_angle * Mathf.Rad2Deg) * dir;
                }
                
                if (has_secondary_projectile)
                {
                    GameManager.Instance.projectileManager.CreateProjectile(
                        projectile.GetSprite(), projectile.GetTrajectory(), 
                        where, dir, projectile.GetSpeed(), OnHitWithSecondaryProjectile, projectile.GetLifetime());
                }
                else
                {
                    GameManager.Instance.projectileManager.CreateProjectile(
                        projectile.GetSprite(), projectile.GetTrajectory(), 
                        where, dir, projectile.GetSpeed(), OnHit, projectile.GetLifetime());
                }
                dir = Quaternion.Euler(0, 0, angle) * dir;
            }

            if (cast_left > 0) yield return new WaitForSeconds(delay);
        }
        
        yield return new WaitForEndOfFrame();
    }


    void OnHitWithSecondaryProjectile(Hittable other, Vector3 impact)
    {
        OnHit(other, impact);

        int total_shots = (int)CalculateProperty(N);
        int shot_left = total_shots;
        Vector3 dir = Vector3.up;
        while (shot_left > 0)
        {
            shot_left--;
            
            GameManager.Instance.projectileManager.CreateProjectile(
                secondary_projectile.GetSprite(), secondary_projectile.GetTrajectory(), 
                impact, dir, secondary_projectile.GetSpeed(), OnHit, secondary_projectile.GetLifetime());
            dir = Quaternion.Euler(0, 0, 360f / total_shots) * dir;
        }
    }
    
    void OnHit(Hittable other, Vector3 impact)
    {
        if (other.team != team)
        {
            if (stun_time > 0 && last_stun_time + stun_time * 2 < Time.time)
            {
                if (other.parent is EnemyController enemy)
                {
                    enemy.Freeze(stun_time);
                }
                last_stun_time = Time.time;
            }
            other.Damage(new Damage(GetDamage(), GetDamageType()));
        }
    }

    void CreateSecondaryProjectile(Hittable other, Vector3 impact)
    {
        
    }
}
