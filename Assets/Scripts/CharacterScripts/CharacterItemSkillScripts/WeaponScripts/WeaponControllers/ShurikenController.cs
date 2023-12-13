using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShurikenController : ItemController
{

    protected override void Launch()
    {
        base.Launch();

        GameObject projectile = Instantiate(ItemData.ProjectileItemPrefab, transform);
        Rigidbody2D projectileRB = projectile.GetComponent<Rigidbody2D>();
        ItemBehaviour projectileBehaviour = projectileRB.GetComponent<ItemBehaviour>();
        projectileBehaviour.InitializeValue(_currentDamage, _currentDeathtime, _currentPierce, _currentSpeed, CurrentWeaponLevel, _currentSizeScale);
        projectileRB.AddForce(PlayerControl.Instance.PlayerLineOfSight * 1000);
    }
}