using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurikenWeaponScript : WeaponScript
{
    private void Awake()
    {
        _damage = 5f;
        _deathTime = 3f;
    }
}
