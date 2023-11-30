using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YRGunController : ItemController
{
    // Start is called before the first frame update

    protected override void Launch()
    {
        base.Launch();

        StartCoroutine(FireGun());

    }

    IEnumerator FireGun()
    {
        for (int i = 0; i < _projectileNum; i++)
        {
            GameObject projectile = Instantiate(ItemData.ProjectileItemPrefab, transform);
            Rigidbody2D projectileRB = projectile.GetComponent<Rigidbody2D>();
            ItemBehaviour projectileBehaviour = projectileRB.GetComponent<ItemBehaviour>();
            projectileBehaviour.InitializeValue(_currentDamage, _currentDeathtime, _currentPierce, _currentSpeed, CurrentWeaponLevel, _currentSizeScale);
            projectileRB.AddForce(PlayerControl.Instance.PlayerLineOfSight * 1500);

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
        SetPierceLimit(9999);
    }
}
