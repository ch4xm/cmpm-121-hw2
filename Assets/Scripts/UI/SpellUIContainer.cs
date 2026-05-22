using Mono.Cecil.Cil;
using UnityEngine;

public class SpellUIContainer : MonoBehaviour
{
    public SpellUI[] spellSlots;
    public PlayerController player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spellSlots = gameObject.GetComponentsInChildren<SpellUI>(true);
        spellSlots[0].highlight.SetActive(true);
        // hide all spells until game starts
        for (int i = 1; i < spellSlots.Length; ++i)
        {
            spellSlots[i].gameObject.SetActive(false);
            spellSlots[i].highlight.SetActive(false);
        }
    }

    public void RefreshUI()
    {
        if (player.spellcaster is null)
        {
            return;
        }
        for (int i = 0; i < spellSlots.Length; i++)
        {
            Spell spell = player.spellcaster.Spells[i];
            SpellUI slot = spellSlots[i];

            if (spell is not null)
            {
                slot.SetSpell(spell);
            }

            slot.gameObject.SetActive(spell is not null);
            slot.highlight.SetActive(i == player.spellcaster.CurrentSpellIndex);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
