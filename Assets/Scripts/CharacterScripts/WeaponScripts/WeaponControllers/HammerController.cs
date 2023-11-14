using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerController : ItemController
{
    protected override void Attack()
    {
        base.Attack();

        GameObject projectile = Instantiate(WeaponData.ItemPrefab, transform);
        ItemBehaviour projectileBehaviour = projectile.GetComponent<ItemBehaviour>();
        projectileBehaviour.InitializeValue(_currentDamage, WeaponData.Deathtime, _currentPierce, _currentSpeed, CurrentWeaponLevel);
    }

    protected override void Level3Effect()
    {
        _currentCooldownDuration *= 0.8f;
    }
}
