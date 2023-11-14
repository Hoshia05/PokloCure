using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordController : ItemController
{
    protected override void Attack()
    {
        base.Attack();

        GameObject projectile = Instantiate(WeaponData.ItemPrefab, transform);


        ItemBehaviour projectileBehaviour = projectile.GetComponent<ItemBehaviour>();
        projectileBehaviour.InitializeValue(_currentDamage, WeaponData.Deathtime, _currentPierce, _currentSpeed, CurrentWeaponLevel);

        float angle = Mathf.Atan2(PlayerControl.Instance.PlayerLineOfSight.y, PlayerControl.Instance.PlayerLineOfSight.x) * Mathf.Rad2Deg;
        projectile.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

    }
}
