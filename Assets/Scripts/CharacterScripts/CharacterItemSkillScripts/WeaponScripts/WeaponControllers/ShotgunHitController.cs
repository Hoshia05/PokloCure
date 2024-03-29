using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunHitController : ItemController
{
    private float _radius = 10f;

    protected override void Launch()
    {
        base.Launch();


        GameObject projectile = Instantiate(ItemData.ProjectileItemPrefab, transform);
        Shockwave();

    }

    private void Shockwave()
    {
        Vector2 playerPosition = transform.position;

        Collider2D[] Enemies = Physics2D.OverlapCircleAll(playerPosition, _radius);

        if (Enemies.Length > 0)
        {
            foreach (Collider2D enemy in Enemies)
            {
                GameObject enemyObject = enemy.gameObject;
                if (enemyObject.CompareTag("Enemy"))
                {
                    Vector2 EnemyPosition = enemyObject.transform.position;

                    Vector2 launchVector = (EnemyPosition - playerPosition).normalized;

                    EnemyScript enemyScript = enemyObject.GetComponent<EnemyScript>();

                    enemyScript.TakeDamage(_currentDamage, _currentKnockbackValue);

                }

            }
        }

    }

    protected override void Level2Effect()
    {
        IncreaseKnockBack(0.5f);
    }

    protected override void Level3Effect()
    {
        IncreaseDamagePercentage(0.5f);
    }

    protected override void Level4Effect()
    {
        DecreaseCooldownPercentage(0.3f);
    }
    protected override void Level5Effect()
    {
        _radius = 20f;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue ;
        Gizmos.DrawWireSphere(transform.position, _radius);
    }
}
