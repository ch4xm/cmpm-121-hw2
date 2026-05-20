using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SpellCaster 
{
    public int mana;
    public int max_mana;
    public int mana_reg;

    public int spell_power;

    public Hittable.Team team;

    private SpellBuilder builder;

    private readonly List<Spell> spells;

    public List<Spell> Spells => spells;

    private int selectedSpellIndex = 0;
    public Spell CurrentSpell =>
        spells[selectedSpellIndex];

    public int CurrentSpellIndex => selectedSpellIndex;


    public SpellUIContainer spellUI;

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

        spellUI = GameManager.Instance.player.GetComponent<PlayerController>().spellUI;

        this.mana = mana;
        this.max_mana = mana;
        this.mana_reg = mana_reg;
        this.team = team;
        this.spell_power = 0;

        this.spells = Enumerable.Repeat<Spell>(null, spellUI.spellSlots.Count()).ToList();

        var spell = builder.Build("arcane_bolt");

        var spell2 = builder.BuildRandomSpell();
        var spell3 = builder.BuildRandomSpell();
        var spell4 = builder.BuildRandomSpell();

        SetSpell(0, spell);
        SetSpell(1, spell2);
        SetSpell(2, spell3);
        SetSpell(3, spell4);

        // Add default spell
    }

    //private void RefreshSpellUI()
    //{
    //    for (int i = 0; i < SpellUI.Count; i++)
    //    {
    //        if (i < spellcaster.equippedSpells.Count)
    //        {
    //            spellui[i].SetSpell(spellcaster.equippedSpells[i]);
    //        }
    //        else
    //        {
    //            spellui[i].SetSpell(null);  // Spell doesn't exist
    //        }

    //        spellui[i].gameObject.SetActive(true);
    //        spellui[i].highlight.SetActive(true);
    //        //spellui[i].highlight.SetActive(i == spellcaster.selectedSpellIndex);
    //    }
    //}

    public void SelectSpell(int index)
    {
        if (index < 0 || index >= spells.Count || spells[index] is null)
            return;

        selectedSpellIndex = index;

        if (spellUI.player.spellcaster is not null)
            spellUI.RefreshUI();
    }


    public void SetSpell(int index, Spell spell)
    {
        if (index < 0 || index >= spells.Count)
            return;

        spells[index] = spell;

        if (spellUI.player.spellcaster is not null)
            spellUI.RefreshUI();
    }

    public IEnumerator Cast(Vector3 where, Vector3 target)
    {
        // Modifiers modifiersContext = new();
        if (mana >= CurrentSpell.GetManaCost() && CurrentSpell.IsReady())
        {
            mana -= CurrentSpell.GetManaCost();
            yield return CurrentSpell.Cast(where, target, team);
        }
        yield break;
    }

}
