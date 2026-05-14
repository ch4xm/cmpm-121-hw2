using System.Collections;
using UnityEngine;
class StatAmplifier : ModifierSpell
{
    public StatAmplifier(Spell innerSpell, ModifierData modifierData) : base(innerSpell)
    {
        
    }

    public override IEnumerator Cast(Vector3 where, Vector3 target, Hittable.Team team, Modifiers modifiersContext)
    {

        yield return innerSpell.Cast(target, where, team, modifiersContext);
    }
    
}
