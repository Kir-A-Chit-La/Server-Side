using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Player", menuName = "Custom/Entity/Player")]
public class Player : ScriptableObject
{
    public int accountId;
    public string accountName;
    [Space]
    public string characterName;
    public string characterGender;
    [Space]
    public Stat health;
}
