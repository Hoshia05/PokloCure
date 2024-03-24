using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerController : ItemController
{
    protected override void Launch()
    {
        base.Launch();

        StartCoroutine(SpawnProjectile());

    }

    IEnumerator SpawnProjectile()
    {
        for (int i = 0; i < _projectileNum; i++)
        {
            ItemBehaviour projectileBehaviour = InstantiateProjectile();

            yield return new WaitForSeconds(0.1f);
        }

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
        AddPierceLimit(9999);
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
