using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunController : ItemController
{
    private int _shotNums = 1;

    // Start is called before the first frame update
    protected override void Launch()
    {
        base.Launch();

        StartCoroutine(FireGun());

    }

    private Vector2 CreateSpread(Vector2 characterLOS)
    {
        float xVariance = (float)(GameManager.Instance.Rand.NextDouble() - 0.5) * 0.4f;
        float yVariance = (float)(GameManager.Instance.Rand.NextDouble() - 0.5) * 0.4f;


        return new Vector2(characterLOS.x + xVariance, characterLOS.y + yVariance);
    }

    IEnumerator FireGun()
    {
        for (int i = 0; i < _projectileNum; i++)
        {
            GameObject projectile = Instantiate(ItemData.ProjectileItemPrefab, transform);
            Rigidbody2D projectileRB = projectile.GetComponent<Rigidbody2D>();
            ItemBehaviour projectileBehaviour = projectileRB.GetComponent<ItemBehaviour>();
            projectileBehaviour.InitializeValue(this, _currentDamage, _currentDeathtime, _currentPierce, _currentSpeed, CurrentWeaponLevel, _currentSizeScale, _currentKnockbackValue);
            projectileRB.AddForce(CreateSpread(PlayerControl.Instance.PlayerLineOfSight) * 2500);
        }

        if(_shotNums == 1)
        {
            yield return null;
        }
        else
        {
            yield return new WaitForSeconds(0.4f);

            for (int i = 0; i < _projectileNum; i++)
            {
                GameObject projectile = Instantiate(ItemData.ProjectileItemPrefab, transform);
                Rigidbody2D projectileRB = projectile.GetComponent<Rigidbody2D>();
                ItemBehaviour projectileBehaviour = projectileRB.GetComponent<ItemBehaviour>();
                projectileBehaviour.InitializeValue(this, _currentDamage, _currentDeathtime, _currentPierce, _currentSpeed, CurrentWeaponLevel, _currentSizeScale, _currentKnockbackValue);
                projectileRB.AddForce(CreateSpread(PlayerControl.Instance.PlayerLineOfSight) * 2500);
            }
        }
    }

    protected override void Level2Effect()
    {
        IncreaseSizePercentage(0.3f);
        IncreaseKnockBack(0.5f);
    }

    protected override void Level3Effect()
    {
        _additionalProjectiles = 3;
    }
    protected override void Level4Effect()
    {
        IncreaseDamagePercentage(1f);
    }
    protected override void Level5Effect()
    {
        _shotNums = 2;
    }

}
