using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackProfile : ScriptableObject
{
    public float AttackDamage;
    public float Cooldown;
    public AttackType type;



}

public enum AttackType
{
    RANGED,
    MELEE,
    SPECIAL,
}
