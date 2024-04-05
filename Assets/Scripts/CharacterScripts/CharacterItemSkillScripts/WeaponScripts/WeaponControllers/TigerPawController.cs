using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TigerPawController : ItemController
{
    private float _hitCooldown = 1f;
    // Start is called before the first frame update
    protected override void Launch()
    {
        base.Launch();

        ItemBehaviour projectileBehaviour = InstantiateProjectile(true);

        projectileBehaviour.SetHitCooldown(_hitCooldown);
    }


    protected override void Level2Effect()
    {
        IncreaseSizePercentage(0.2f);

    }

    protected override void Level3Effect()
    {
        DecreaseCooldownPercentage(0.3f);
    }

    protected override void Level4Effect()
    {
        IncreaseSizePercentage(0.25f);
    }
    protected override void Level5Effect()
    {
        _stunTime = 0.2f;
    }

}
