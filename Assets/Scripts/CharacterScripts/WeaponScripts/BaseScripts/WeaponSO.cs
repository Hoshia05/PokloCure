using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "WeaponSO", menuName = "Scriptable Object/WeaponSO")]
public class WeaponSO : ScriptableObject
{
    public GameObject Prefab;
    public float Damage;
    public float Speed;
    public float CooldownDuration;
    public int Pierce = 1;
}
