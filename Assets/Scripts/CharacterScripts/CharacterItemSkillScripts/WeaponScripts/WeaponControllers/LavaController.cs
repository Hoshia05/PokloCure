using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaController : ItemController
{
    protected override void Launch()
    {
        base.Launch();

        StartCoroutine(BucketThrowCoroutine());
    }

    IEnumerator BucketThrowCoroutine()
    {
        for(int i = 0; i < _projectileNum; i++)
        {
            ItemBehaviour projectileBehaviour = InstantiateProjectile();

            projectileBehaviour.SetHitCooldown(0.75f);

            yield return new WaitForSeconds(0.1f);
        }
    }


    protected override void Level2Effect()
    {
        IncreaseSizePercentage(0.2f);
    }

    protected override void Level3Effect()
    {
        _additionalProjectiles++;
    }

    protected override void Level4Effect()
    {
        IncreaseDamagePercentage(0.3f);
        IncreaseDeathTime(1f);

    }
    protected override void Level5Effect()
    {
        IncreaseDamagePercentage(0.3f);
    }
    protected override void Level6Effect()
    {
        _additionalProjectiles++;
    }

    protected override void Level7Effect()
    {
        _additionalProjectiles++;
        IncreaseSizePercentage(0.2f);
    }
}
