using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShurikenController : ItemController
{

    protected override void Launch()
    {
        base.Launch();

        //GameObject projectile = Instantiate(ItemData.ProjectileItemPrefab, transform);
        //Rigidbody2D projectileRB = projectile.GetComponent<Rigidbody2D>();
        //ItemBehaviour projectileBehaviour = projectileRB.GetComponent<ItemBehaviour>();
        //projectileBehaviour.InitializeValue(this, _currentDamage, _currentDeathtime, _currentPierce, _currentSpeed, CurrentLevel, _currentSizeScale, _currentKnockbackValue);

        ItemBehaviour projectileBehaviour = InstantiateProjectile();

        Rigidbody2D projectileRB = projectileBehaviour.GetComponent<Rigidbody2D>();
        projectileRB.AddForce(PlayerControl.Instance.PlayerLineOfSight * 1000);
    }
}
