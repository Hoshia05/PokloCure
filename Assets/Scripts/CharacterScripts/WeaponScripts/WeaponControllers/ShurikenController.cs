using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShurikenController : ItemController
{

    protected override void Attack()
    {
        base.Attack();

        GameObject projectile = Instantiate(WeaponData.ItemPrefab, transform);
        Rigidbody2D projectileRB = projectile.GetComponent<Rigidbody2D>();
        ItemBehaviour projectileBehaviour = projectileRB.GetComponent<ItemBehaviour>();
        projectileBehaviour.InitializeValue(_currentDamage, WeaponData.Deathtime, _currentPierce, _currentSpeed, CurrentWeaponLevel);
        projectileRB.AddForce(PlayerControl.Instance.PlayerLineOfSight * 1000);
    }
}
