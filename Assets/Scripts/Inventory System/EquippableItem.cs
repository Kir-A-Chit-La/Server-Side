using UnityEngine;

public enum EquipmentType
{
    Headdress,
    Outerwear,
    Backpack,
    Underwear,
    Unavailable1,
    Unavailable2,
    Weapon,
    Pants,
    Reserve,
    Shoes
}

[CreateAssetMenu(fileName = "New Equippable Item", menuName = "Custom/Items/Equippable Item")]
public class EquippableItem : Item
{
    public EquipmentType EquipmentType;
    [Space]
    public int StrengthFlat;
    public int ArmourFlat;
    public int FreezeResistanceFlat;
    public int HeatResistanceFlat;
    public int BackpackSize;
    [Space]
    public float StrengthPercentage;
    public float ArmourPercentage;
    public float FreezeResistancePercentage;
    public float HeatResistancePercentage;

    public override Item GetCopy() => Instantiate(this);
    public override void Destroy() => Destroy(this);
    public void Equip(PlayerStats player)
    {
        if(StrengthFlat != 0)
            player.Strength.AddModifier(new StatModifier(StrengthFlat, StatModifierType.Flat, this));
        if(ArmourFlat != 0)
            player.Armour.AddModifier(new StatModifier(ArmourFlat, StatModifierType.Flat, this));
        if(FreezeResistanceFlat != 0)
            player.FreezeResistance.AddModifier(new StatModifier(FreezeResistanceFlat, StatModifierType.Flat, this));
        if(HeatResistanceFlat != 0)
            player.HeatResistance.AddModifier(new StatModifier(HeatResistanceFlat, StatModifierType.Flat, this));
        if(BackpackSize != 0)
            player.BackpackSize.AddModifier(new StatModifier(BackpackSize, StatModifierType.Flat, this));

        if(StrengthPercentage != 0)
            player.Strength.AddModifier(new StatModifier(StrengthPercentage, StatModifierType.PercentageMultiplicative, this));
        if(ArmourPercentage != 0)
            player.Armour.AddModifier(new StatModifier(ArmourPercentage, StatModifierType.PercentageMultiplicative, this));
        if(FreezeResistancePercentage != 0)
            player.FreezeResistance.AddModifier(new StatModifier(FreezeResistancePercentage, StatModifierType.PercentageMultiplicative, this));
        if(HeatResistancePercentage != 0)
            player.HeatResistance.AddModifier(new StatModifier(HeatResistancePercentage, StatModifierType.PercentageMultiplicative, this));
    }
    public void Unequip(PlayerStats player)
    {
        player.Strength.RemoveAllModifiersFromSource(this);
        player.Armour.RemoveAllModifiersFromSource(this);
        player.FreezeResistance.RemoveAllModifiersFromSource(this);
        player.HeatResistance.RemoveAllModifiersFromSource(this);
        player.BackpackSize.RemoveAllModifiersFromSource(this);
    }
}
