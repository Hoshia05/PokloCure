using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponProfile : MonoBehaviour
{
    public float Damage;
    public float Cooldown;
    public GameObject AttackPrefab;

    private WeaponScript _weaponScript;

    private void Awake()
    {
        _weaponScript = AttackPrefab.GetComponent<WeaponScript>();
    }

    public abstract void AttackPattern();

}
