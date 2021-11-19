using System;
using UnityEngine;

[Serializable, CreateAssetMenu(fileName = "Damage Resistance", menuName = "Data/Gameplay/Damage/Damage Resistance")]
public class DamageResistance : ScriptableObject
{
    public DamageType resistantTo;
}