using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordController : ItemController
{
    protected override void Launch()
    {
        base.Launch();

        GameObject projectile = Instantiate(ItemData.ProjectileItemPrefab, transform);


        ItemBehaviour projectileBehaviour = projectile.GetComponent<ItemBehaviour>();
        projectileBehaviour.InitializeValue(_currentDamage, _currentDeathtime, _currentPierce, _currentSpeed, CurrentWeaponLevel, _currentSizeScale);

        float angle = Mathf.Atan2(PlayerControl.Instance.PlayerLineOfSight.y, PlayerControl.Instance.PlayerLineOfSight.x) * Mathf.Rad2Deg;
        projectile.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

    }
}
