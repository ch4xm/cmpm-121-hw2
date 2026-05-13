using UnityEngine;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;

using System;
using JetBrains.Annotations;


public class SpellBuilder 
{
    private Dictionary<string, SpellData> spells;

    public Spell Build(SpellCaster owner)
    {
        // for testing
        var test_modifier = new ModifierData();
        test_modifier.name = "test_modifier";
        test_modifier.damage_multiplier = "2";
        test_modifier.projectile_trajectory = "spiraling";
        ModifierData[] modifiers = new ModifierData[] { test_modifier };
        
        return new Spell(owner, spells["arcane_bolt"], modifiers) ; // Defaults to bolt for now, need to make this work with UI
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
    public string mana_cost;
    public float cooldown;
    public Projectile projectile;
    public Projectile secondary_projectile;
}

// Helper classes for parsing modifiers from JSON
public class ModifierData
{
    public string name;
    public string description;
    [CanBeNull] public string damage_multiplier;
    [CanBeNull] public string mana_multiplier;
    [CanBeNull] public string cooldown_multiplier;
    [CanBeNull] public string speed_multiplier;
    [CanBeNull] public string angle;
    [CanBeNull] public string delay;
    [CanBeNull] public string projectile_trajectory;
    [CanBeNull] public string mana_adder;
}

public class DamageData
{
    public string amount;
    public string type;
}

//public class ProjectileData
//{
//    public int sprite;
//    public string trajectory;
//    public string speed;
//    public float? lifetime;
//}