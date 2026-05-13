using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static UnityEngine.UI.CanvasScaler;

public class Modifiers
{
    public List<ValueModifier> damageModifiers = new ();
    public List<ValueModifier> speedModifiers = new ();
    public List<ValueModifier> manaModifiers = new ();
    public List<ValueModifier> cooldownModifiers = new();

    public string? trajectoryOverride = null;
}

abstract class ModifierSpell : Spell
{
    protected Spell innerSpell;

    public ModifierSpell(Spell innerSpell) : base(innerSpell.owner)
    {
        this.innerSpell = innerSpell;
    }


    //public abstract IEnumerator Cast(Vector3 where, Vector3 target, Hittable.Team team);

}
