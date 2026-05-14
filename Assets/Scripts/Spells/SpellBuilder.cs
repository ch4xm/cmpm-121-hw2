using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Unity.Mathematics;
using UnityEngine;
using static DataLoader;
using Random = UnityEngine.Random;
using NUnit.Framework;


public class SpellBuilder
{
    private readonly SpellCaster owner;
    private readonly Dictionary<string, SpellData> spells;
    private readonly Dictionary<string, ModifierData> modifiers;

    private List<string> spellKeys => spells.Keys.ToList();
    private List<string> modifierKeys => modifiers.Keys.ToList();


    public Spell Build(string spellName, List<string> modifierNames = null) // if no modifiers passed just make basic spell
    {
        SpellData spellData = spells[spellName];


        // Map list of modifier names to modifierdata classes
        List<ModifierData> selectedModifiers = new();
        modifierNames?.ForEach(key => selectedModifiers.Add(modifiers[key]));
        
        var spell = new Spell(owner, spellData, selectedModifiers); // Defaults to bolt for now, need to make this work with UI
        return spell;
    }

    public Spell BuildRandomSpell()
    {
        string key = spellKeys[Random.Range(0, spellKeys.Count)];
        SpellData randomBaseSpell = spells[key];

        List<ModifierData> result = new();


        int modifierCount = Random.Range(1, 4);
        for (int i = 0; i < Math.Min(modifierCount, modifierKeys.Count); i++)
        {
            string modifierKey = modifierKeys[Random.Range(0, modifierKeys.Count)];
            ModifierData data = modifiers[modifierKey];

            if (!result.Contains(data))
            {
                result.Add(data);
            }
        }

        var randomSpell = new Spell(owner, randomBaseSpell, result);
        return randomSpell;
    }

    //private Spell CreateBaseSpell(string key)
    //{
    //    switch (key) {
    //        case "arcane_bolt":
    //            return new Spell(owner, spells[key]);
    //        default:
    //            throw new Exception($"Unknown spell: {key}");
    //    }
    //}

    /*
    private ModifierSpell CreateModifierSpell(string key, Spell innerSpell)
    {
        if (!modifiers.ContainsKey(key))
        {
            throw new Exception($"Unknown modifier key {key}");
        }
        ModifierData modifierData = modifiers[key];

        switch (key)
        {

            case "doubler":
                return new StatAmplifier(innerSpell, modifierData);

            case "splitter":
                return new StatAmplifier(innerSpell, modifierData);

            case "chaos":
                return new StatAmplifier(innerSpell, modifierData);

            case "homing":
                return new StatAmplifier(innerSpell, modifierData);


            case "damage_amp":
            case "speed_amp":
            default: // default to basic modifierspell with no custom behavior
                return new StatAmplifier(innerSpell, modifierData);
                //throw new Exception($"Unimplemented modifier with key {key}");
        }
    }
    */

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
    public string N;
    public string spray;
    public string mana_cost;
    public string cooldown;
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
    public string? multicast;
    public string? multishot;
    public string? delay;
    public string? projectile_trajectory;
    public string? mana_adder;
    public string? time;
}

public class DamageData
{
    public string amount;
    public string type;
}