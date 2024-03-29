using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlailController : ItemController
{
    private void Start()
    {
    }

    protected override void Launch()
    {
        ItemBehaviour projectileBehaviour = InstantiateProjectile();

        float angle = Mathf.Atan2(PlayerControl.Instance.PlayerLineOfSight.y, PlayerControl.Instance.PlayerLineOfSight.x) * Mathf.Rad2Deg;
        projectileBehaviour.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        //GameObject projectile = Instantiate(ItemData.ProjectileItemPrefab, transform);
        //ItemBehaviour projectileBehaviour = projectile.GetComponent<ItemBehaviour>();
        //projectileBehaviour.InitializeValue(this, _currentDamage, _currentDeathtime, _currentPierce, _currentSpeed, CurrentLevel, _currentSizeScale, _currentKnockbackValue);

        //float angle = Mathf.Atan2(PlayerControl.Instance.PlayerLineOfSight.y, PlayerControl.Instance.PlayerLineOfSight.x) * Mathf.Rad2Deg;
        //projectile.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        if(_projectileNum == 2)
        {

            ItemBehaviour projectileBehaviour2 = InstantiateProjectile();

            projectileBehaviour2.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle + 180f));

            //GameObject projectile2 = Instantiate(ItemData.ProjectileItemPrefab, transform);
            //ItemBehaviour projectileBehaviour2 = projectile2.GetComponent<ItemBehaviour>();
            //projectileBehaviour2.InitializeValue(this, _currentDamage, _currentDeathtime, _currentPierce, _currentSpeed, CurrentLevel, _currentSizeScale, _currentKnockbackValue);

            //projectile2.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle + 180f));

        }

    }

    protected override void Level2Effect()
    {
        IncreaseSpeedPercentage(0.2f);
    }

    protected override void Level3Effect()
    {
        IncreaseSizePercentage(0.3f);
        AddPierceLimit(5);
    }

    protected override void Level4Effect()
    {
        IncreaseDamagePercentage(0.3f);
        IncreaseSizePercentage(0.2f);
    }
    protected override void Level5Effect()
    {
        _additionalProjectiles = 1;
    }
    protected override void Level6Effect()
    {
        AddPierceLimit(5);
    }

    protected override void Level7Effect()
    {
        IncreaseKnockBack(2);
    }
}
