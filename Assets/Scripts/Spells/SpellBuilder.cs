using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static DataLoader;


public class SpellBuilder 
{
    private Dictionary<string, SpellData> spells;
    private Dictionary<string, ModifierData> modifiers;

    public Spell Build(SpellCaster owner)
    {
        // for testing
        var test_modifier = new ModifierData();
        test_modifier.name = "test_modifier";
        test_modifier.damage_multiplier = "2";
        test_modifier.projectile_trajectory = "spiraling";
        ModifierData[] selectedModifiers = new ModifierData[] { modifiers["homing"], modifiers["speed_amp"] };
        
        
        return new Spell(owner, spells["arcane_bolt"], selectedModifiers) ; // Defaults to bolt for now, need to make this work with UI
    }

   
    public SpellBuilder()
    {
        SpellReadResult result = ReadSpells();

        spells = result.Spells;
        modifiers = result.Modifiers;
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
    public string? damage_multiplier;
    public string? mana_multiplier;
    public string? cooldown_multiplier;
    public string? speed_multiplier;
    public string? angle;
    public string? delay;
    public string? projectile_trajectory;
    public string? mana_adder;
}

public class DamageData
{
    public string amount;
    public string type;
}