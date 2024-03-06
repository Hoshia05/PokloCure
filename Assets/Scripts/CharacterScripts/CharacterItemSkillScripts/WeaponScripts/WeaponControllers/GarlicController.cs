using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarlicController : ItemController
{
    private float _hitCooldown = 1f;

    // Start is called before the first frame update
    protected override void Launch()
    {
        base.Launch();

        ItemBehaviour projectileBehaviour = InstantiateProjectile();
        projectileBehaviour.SetHitCooldown(_hitCooldown);
    }

    protected override void LevelUpEffect()
    {
        ResetProjectiles();
    }


    protected override void Level2Effect()
    {
        IncreaseSizePercentage(0.15f);
    }

    protected override void Level3Effect()
    {
        IncreaseDamagePercentage(0.3f);
    }

    protected override void Level4Effect()
    {
        IncreaseSizePercentage(0.25f);
    }
    protected override void Level5Effect()
    {
        _hitCooldown = 0.6f;
    }
    protected override void Level6Effect()
    {
        IncreaseDamagePercentage(0.6f);
    }

    protected override void Level7Effect()
    {
        _currentKnockbackValue += 5f;
    }
}


