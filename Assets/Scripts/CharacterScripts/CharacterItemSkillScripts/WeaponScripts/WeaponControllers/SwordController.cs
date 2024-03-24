using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordController : ItemController
{
    protected override void Launch()
    {
        base.Launch();

        //GameObject projectile = Instantiate(ItemData.ProjectileItemPrefab, transform);

        //ItemBehaviour projectileBehaviour = projectile.GetComponent<ItemBehaviour>();
        //projectileBehaviour.InitializeValue(this, _currentDamage, _currentDeathtime, _currentPierce, _currentSpeed, CurrentLevel, _currentSizeScale, _currentKnockbackValue);


        ItemBehaviour projectileBehaviour = InstantiateProjectile();

        Vector2 AttackDirection = FindClosestEnemy();

        float angle = Mathf.Atan2(AttackDirection.y, AttackDirection.x) * Mathf.Rad2Deg;
        projectileBehaviour.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

    }

    private Vector2 FindClosestEnemy()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, 10f, LayerMask.GetMask("Enemy"));

        Vector2 BestDirection;
        EnemyScript BestEnemy = null;
        float CurrentClosestDistance = 0;

        if(hitColliders.Length == 0 )
        {
            BestDirection = PlayerControl.Instance.PlayerLineOfSight;
        }
        else
        {
            foreach(Collider2D collider in hitColliders)
            {
                if (collider.gameObject.CompareTag("Enemy") == false)
                    continue;

                GameObject enemy = collider.gameObject;
                EnemyScript eScript = enemy.GetComponent<EnemyScript>();
                float enemyDistance = (enemy.transform.position - transform.position).magnitude;

                if (BestEnemy == null)
                {
                    BestEnemy = eScript;
                    CurrentClosestDistance = enemyDistance;
                }
                else
                {
                    if(BestEnemy.EClass < eScript.EClass)
                    {
                        BestEnemy = eScript;
                        CurrentClosestDistance = enemyDistance;
                    }else if(BestEnemy.EClass == eScript.EClass && enemyDistance <= CurrentClosestDistance)
                    {
                        BestEnemy = eScript;
                        CurrentClosestDistance = enemyDistance;
                    }
                }


            }

            BestDirection = (BestEnemy.transform.position - transform.position).normalized;
        }

        return BestDirection;
    }

    protected override void Level2Effect()
    {
        IncreaseDamagePercentage(0.1f);
    }

    protected override void Level3Effect()
    {
        AddPierceLimit(9999);
    }
    protected override void Level4Effect()
    {
        IncreaseKnockBack(0.5f);
    }

    protected override void Level5Effect()
    {
        DecreaseCooldownPercentage(0.15f);
    }
}
