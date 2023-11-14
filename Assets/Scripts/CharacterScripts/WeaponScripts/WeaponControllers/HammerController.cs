using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerController : ItemController
{
    protected override void Attack()
    {
        base.Attack();

        GameObject projectile = Instantiate(ItemData.ItemPrefab, transform);
        ItemBehaviour projectileBehaviour = projectile.GetComponent<ItemBehaviour>();
        projectileBehaviour.InitializeValue(_currentDamage, _currentDeathtime, _currentPierce, _currentSpeed, CurrentWeaponLevel, _currentSizeScale);
    }

    protected override void Level2Effect()
    {
        _currentSizeScale *= 1.2f;
        _currentDamage *= 1.2f;
    }

    protected override void Level3Effect()
    {
        _currentCooldownDuration *= 0.8f;
    }

    protected override void Level4Effect()
    {
        _currentSizeScale *= 1.2f;
        _currentDamage *= 1.33f;
    }
    protected override void Level5Effect()
    {
        _currentPierce = 9999;
        _currentDeathtime += 1f;
    }
    protected override void Level6Effect()
    {
        _currentSizeScale *= 1.5f;
    }

    protected override void Level7Effect()
    {
        _currentDamage *= 1.5f;
    }

}
