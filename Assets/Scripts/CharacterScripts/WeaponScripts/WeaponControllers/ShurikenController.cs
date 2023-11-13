using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShurikenController : WeaponController
{

    protected override void Attack()
    {
        base.Attack();

        GameObject projectile = Instantiate(WeaponData.Prefab, transform);
        Rigidbody2D projectileRB = projectile.GetComponent<Rigidbody2D>();
        WeaponBehaviour projectileBehaviour = projectileRB.GetComponent<WeaponBehaviour>();
        projectileBehaviour.InitializeValue(_currentDamage, Deathtime, _currentPierce);
        projectileRB.AddForce(PlayerControl.Instance.PlayerLineOfSight * 1000);
    }
}
