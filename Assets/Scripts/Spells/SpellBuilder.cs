using UnityEngine;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;

using System;


public class SpellBuilder 
{
    private Dictionary<string, SpellData> spells;

    public Spell Build(SpellCaster owner)
    {
        return new Spell(owner, spells["arcane_bolt"]); // Defaults to bolt for now, need to make this work with UI
    }

   
    public SpellBuilder()
    {
        spells = DataLoader.ReadSpells();
        //Console.WriteLine(spells);
    }



}

// Helper classes for parsing spells from JSON
public class SpellData
{
    public string name;
    public string description;
    public int icon;
    public DamageData damage;
    public int mana_cost;
    public float cooldown;
    public ProjectileData projectile;
    public ProjectileData secondary_projectile;
}

public class DamageData
{
    public string amount;
    public string type;
}

public class ProjectileData
{
    public int sprite;
    public string trajectory;
    public float speed;
    public float? lifetime;
}