using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using static DataLoader;


public class SpellBuilder
{
    private readonly SpellCaster owner;
    private readonly Dictionary<string, SpellData> spells;
    private readonly Dictionary<string, ModifierData> modifiers;

    private List<string> spellKeys => spells.Keys.ToList();
    private List<string> modifierKeys => modifiers.Keys.ToList();


    public Spell Build(string spellName, List<string> modifierNames = null) // if no modifiers passed just make basic spell
    {
        // for testing
        var test_modifier = new ModifierData();
        test_modifier.name = "test_modifier";
        test_modifier.damage_multiplier = "2";
        test_modifier.projectile_trajectory = "spiraling";
        ModifierData[] selectedModifiers = new ModifierData[] { modifiers["homing"], modifiers["speed_amp"] };

        Spell spell = CreateBaseSpell(spellName);

        if (modifierNames?.Count > 0)
        {
            foreach (var modifierKey in modifierNames)
            {
                spell = CreateModifierSpell(modifierKey, spell);
            }
        }
        return spell
        //Spell spell = new Spell(owner, spells["arcane_bolt"], selectedModifiers) ; // Defaults to bolt for now, need to make this work with UI
    }

    public Spell BuildRandomSpell()
    {
    }

    private Spell CreateBaseSpell(string key)
    {
        switch (key) {
            case "arcane_bolt":
                return new Spell(owner, spells[key]);
            default:
                throw new Exception($"Unknown spell: {key}");
        }
    }

    private ModifierSpell CreateModifierSpell(string key, Spell innerSpell)
    {
        if (!modifiers.ContainsKey(key))
        {
            throw new Exception($"Unknown modifier key {key}");
        }
        ModifierData modifierData = modifiers[key];

        switch (key)
        {
            case "damage_amp":
                return new DamageAmplifier(innerSpell, modifierData);
            case "damage_amp":
            case "damage_amp":
            case "damage_amp":
            case "damage_amp":
            default:

        }
    }

    public Spell CreateModifierSpell()
    {

    }
   
    public SpellBuilder(SpellCaster owner)
    {
        SpellReadResult result = ReadSpells();

        spells = result.Spells;
        modifiers = result.Modifiers;
        this.owner = owner;
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

    public static Apply()
}

public class DamageData
{
    public string amount;
    public string type;
}