using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerController : ItemController
{
    protected override void Launch()
    {
        base.Launch();

        GameObject projectile = Instantiate(ItemData.ProjectileItemPrefab, transform);
        ItemBehaviour projectileBehaviour = projectile.GetComponent<ItemBehaviour>();
        projectileBehaviour.InitializeValue(_currentDamage, _currentDeathtime, _currentPierce, _currentSpeed, CurrentWeaponLevel, _currentSizeScale, _currentKnockbackValue);
    }

    protected override void Level2Effect()
    {
        IncreaseDamagePercentage(0.2f);
        IncreaseSizePercentage(0.2f);
    }

    protected override void Level3Effect()
    {
        DecreaseCooldownPercentage(0.2f);
    }

    protected override void Level4Effect()
    {
        IncreaseDamagePercentage(0.33f);
        IncreaseSizePercentage(0.2f);
    }
    protected override void Level5Effect()
    {
        SetPierceLimit(9999);
        IncreaseDeathTime(1f);
    }
    protected override void Level6Effect()
    {
        IncreaseSizePercentage(0.5f);
    }

    protected override void Level7Effect()
    {
        IncreaseDamagePercentage(0.5f);
    }

}
