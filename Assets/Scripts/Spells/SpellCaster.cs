using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpellCaster 
{
    public int mana;
    public int max_mana;
    public int mana_reg;

    public int spell_power;

    public Hittable.Team team;

    private SpellBuilder builder;

    public List<Spell> spells = new ();

    public int selectedSpellIndex = 0;
    public Spell CurrentSpell =>
        spells[selectedSpellIndex];

    public IEnumerator ManaRegeneration()
    {
        while (true)
        {
            mana += mana_reg;
            mana = Mathf.Min(mana, max_mana);
            yield return new WaitForSeconds(1);
        }
    }

    public SpellCaster(int mana, int mana_reg, Hittable.Team team)
    {
        this.builder = new SpellBuilder(this);

        this.mana = mana;
        this.max_mana = mana;
        this.mana_reg = mana_reg;
        this.team = team;
        this.spell_power = 0;

        var testModifiers = new List<string> { "homing", "speed_amp", "damage_amp" };
        var spell = builder.Build("arcane_bolt", testModifiers);
        var spell2 = builder.Build("arcane_bolt");

        spells.Add(spell);  // Add default spell
        spells.Add(spell2);
    }

    public IEnumerator Cast(Vector3 where, Vector3 target)
    {
        Modifiers modifiersContext = new();
        if (mana >= CurrentSpell.GetManaCost() && CurrentSpell.IsReady())
        {
            mana -= CurrentSpell.GetManaCost();
            yield return CurrentSpell.Cast(where, target, team, modifiersContext);
        }
        yield break;
    }

}
