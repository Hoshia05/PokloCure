using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GTGunController : ItemController
{
    // Start is called before the first frame update

    protected override void Launch()
    {
        base.Launch();

        StartCoroutine(FireGun());

    }

    IEnumerator FireGun()
    {
        Vector2 fireVector = PlayerControl.Instance.PlayerLineOfSight;

        for (int i = 0; i < _projectileNum; i++)
        {
            ItemBehaviour projectileBehaviour = InstantiateProjectile();
            Rigidbody2D projectileRB = projectileBehaviour.GetComponent<Rigidbody2D>();
            projectileRB.AddForce(fireVector * 1500);

            yield return new WaitForSeconds(0.05f);
        }
    }

    protected override void Level2Effect()
    {
        _projectileNum++;
    }

    protected override void Level3Effect()
    {
        DecreaseCooldownPercentage(0.2f);
    }

    protected override void Level4Effect()
    {
        IncreaseDamagePercentage(0.20f);
    }
    protected override void Level5Effect()
    {
        IncreaseSizePercentage(0.3f);
    }
    protected override void Level6Effect()
    {
        IncreaseDamagePercentage(0.20f);
    }

    protected override void Level7Effect()
    {
        _projectileNum++;
        _projectileNum++;
        AddPierceLimit(2);
    }
}
