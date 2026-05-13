using System;
using System.Collections.Generic;
using System.Text;

class ValueModifier // Each line in a modifier spell corresponds to one of these, eg "damage_multiplier": "1.5"
{
    public enum ModifierType
    {
        Addition,
        Multiplication
    }

    public ModifierType type;
    public float operand;
    public ValueModifier(ModifierType type, float operand)
    {
        this.type = type;
        this.operand = operand;
    }

    public float Modify(float value)
    {
        if (type == ModifierType.Addition)
        {
            value += operand;
        }
        else if (type == ModifierType.Multiplication)
        {
            value *= operand;
        }

        return value;
    }

    public static float ApplyModifiers(float value, List<ValueModifier> modifiers)
    {
        foreach (ValueModifier modifier in modifiers)
        {
            value = modifier.Modify(value);
        }

        return (float) value;
    }
}

