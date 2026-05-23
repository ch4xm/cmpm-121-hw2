using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

public class SpellCaster 
{
    public int mana;
    public int max_mana;
    public int mana_reg;

    public int spell_power;

    public Hittable.Team team;

    public SpellBuilder builder;

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
        SetSpell(0, spell);
    }


    public void SelectSpell(int index)
    {
        if (index < 0 || index >= spells.Count || spells[index] is null)
            return;

        selectedSpellIndex = index;

            spellUI.RefreshUI();
    }

    public void SetFirstAvailableSpell(Spell spell) // Find first open slot, otherwise if no open slots override first spell to new spell
    {
        for (int i = 0; i < spells.Count; i++)
        {
            if (spells[i] is null)
            {
                spells[i] = spell;
                    spellUI.RefreshUI();
                return;
            }
        }
        spells[0] = spell;
        spellUI.RefreshUI();

    }

    public void DropSpell(int index)
    {
        if (index < 0 || index >= spells.Count)
            return;

        spells[index] = null;
        spellUI.RefreshUI();
    }

    public void SetSpell(int index, Spell spell)
    {
        if (index < 0 || index >= spells.Count)
            return;

        spells[index] = spell;
        spellUI.RefreshUI();
    }

    public IEnumerator Cast(Vector3 where, Vector3 target)
    {
        // Modifiers modifiersContext = new();
        if (mana >= CurrentSpell.GetManaCost() && CurrentSpell.IsReady())
        {
            mana -= CurrentSpell.GetManaCost();
            EventBus.Instance.DoCastSpell(GameManager.Instance.player.GetComponent<PlayerController>().hp);
            yield return CurrentSpell.Cast(where, target, team);
        }
        yield break;
    }

}
