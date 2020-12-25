using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;

[Serializable]
public class Stat
{
    public float BaseValue;
    public virtual float Value
    {
        get
        {
            if(isDirty || BaseValue != _lastBaseValue)
            {
                _lastBaseValue = BaseValue;
                _value = CalculateFinalValue();
                isDirty = false;
            }
            return _value;
        }
    }
    protected float _value;
    protected float _lastBaseValue = float.MinValue;
    protected bool isDirty = true;
    protected bool isRemoved = false;
    protected readonly List<StatModifier> statModifiers;
    public readonly ReadOnlyCollection<StatModifier> StatModifiers;
    public Stat()
    {
        statModifiers = new List<StatModifier>();
        StatModifiers = statModifiers.AsReadOnly();
    }
    public Stat(float baseValue) : this()
    {
        BaseValue = baseValue;
    }
    public virtual void AddModifier(StatModifier modifier)
    {
        isDirty = true;
        statModifiers.Add(modifier);   
        statModifiers.Sort(CompareModifierOrder);
    }
    public virtual bool RemoveModifier(StatModifier modifier)
    {
        if(statModifiers.Remove(modifier))
        {
            isDirty = true;
            return true;
        }
        return false;
    }
    public virtual bool RemoveAllModifiersFromSource(object source)
    {
        isRemoved = false;
        for(int i = statModifiers.Count - 1; i >= 0; i--)
        {
            if(statModifiers[i].Source == source)
            {
                isDirty = true;
                isRemoved = true;
                statModifiers.RemoveAt(i);
            }
        }
        return isRemoved;
    }
    protected virtual int CompareModifierOrder(StatModifier a, StatModifier b)
    {
        if(a.Order < b.Order)
            return -1;
        else if(a.Order > b.Order)
            return 1;
        return 0; // if(a.Order == b.Order)
    }
    protected virtual float CalculateFinalValue()
    {
        float finalValue = BaseValue;
        float sumPercentAdd = 0f;
        for(int i = 0; i < statModifiers.Count; i++)
        {
            StatModifier modifier = statModifiers[i];
            if(modifier.Type == StatModifierType.Flat)
            {
                finalValue += statModifiers[i].Value;
            }
            else if(modifier.Type == StatModifierType.PercentageAdditive)
            {
                sumPercentAdd += modifier.Value;
                if(i + 1 >= statModifiers.Count || statModifiers[i + 1].Type != StatModifierType.PercentageAdditive)
                {
                    finalValue *= 1 + sumPercentAdd;
                    sumPercentAdd = 0f;
                }
            }
            else if(modifier.Type == StatModifierType.PercentageMultiplicative)
            {
                finalValue *= 1 + modifier.Value;
            }
        }
        return (float)Math.Round(finalValue, 4);
    }
}
