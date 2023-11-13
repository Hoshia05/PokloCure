using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurikenBehaviour : WeaponBehaviour
{
    private void Awake()
    {
        _damage = 5f;
        _deathTime = 3f;
    }
}
