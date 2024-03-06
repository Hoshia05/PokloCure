using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordController : ItemController
{
    protected override void Launch()
    {
        base.Launch();

        GameObject projectile = Instantiate(ItemData.ProjectileItemPrefab, transform);


        ItemBehaviour projectileBehaviour = projectile.GetComponent<ItemBehaviour>();
        projectileBehaviour.InitializeValue(this, _currentDamage, _currentDeathtime, _currentPierce, _currentSpeed, CurrentWeaponLevel, _currentSizeScale, _currentKnockbackValue);

        Vector2 AttackDirection = FindClosestEnemy();

        float angle = Mathf.Atan2(AttackDirection.y, AttackDirection.x) * Mathf.Rad2Deg;
        projectile.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

    }

    private Vector2 FindClosestEnemy()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, 20f, LayerMask.GetMask("Enemy"));

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
                    if(enemyDistance <= CurrentClosestDistance)
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
}
