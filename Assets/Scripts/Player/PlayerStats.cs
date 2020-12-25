using UnityEngine;

[CreateAssetMenu(fileName = "New Player Stats", menuName = "Custom/Entity/Player/Stats")]
public class PlayerStats : ScriptableObject
{
    public Stat Health;
    public Stat Strength;
    public Stat Armour;
    public Stat FreezeResistance;
    public Stat HeatResistance;
    public Stat Experience;
    public Stat StandingSpeed;
    public Stat CrouchingSpeed;
    public Stat Stench;
    public Stat Luck;
    public Stat BackpackSize;
}