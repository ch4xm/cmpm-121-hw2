using UnityEngine;
using System;

public class Hittable
{

    public enum Team { PLAYER, MONSTERS }
    public Team team;

    public int hp;
    public int max_hp;

    public GameObject owner;

    public MonoBehaviour parent;
    
    public float lastDamageTime = 0f;
    
    public void Damage(Damage damage)
    {
        EventBus.Instance.DoDamage(owner.transform.position, damage, this);
        hp -= damage.amount;
        lastDamageTime = Time.time;
        if (hp <= 0)
        {
            hp = 0;
            OnDeath();
        }
    }
    
    public void Heal(float amount)
    {
        hp += (int)amount;
        hp = Mathf.Clamp(hp, 0, max_hp);
    }
    
    public event Action OnDeath;

    public Hittable(int hp, Team team, GameObject owner, MonoBehaviour parent)
    {
        this.hp = hp;
        this.max_hp = hp;
        this.team = team;
        this.owner = owner;

        this.parent = parent;
    }

    public void SetMaxHP(int max_hp)
    {
        float perc = this.hp * 1.0f / this.max_hp;
        this.max_hp = max_hp;
        this.hp = Mathf.RoundToInt(perc * max_hp);
    }
}
