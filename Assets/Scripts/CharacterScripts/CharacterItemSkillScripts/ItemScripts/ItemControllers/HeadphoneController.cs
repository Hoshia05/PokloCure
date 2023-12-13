using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadphoneController : ItemController
{
    public float NegateChance = 0.15f;


    protected void Start()
    {
        PlayerScript.Instance.OnDamageTakenBool += TriggerNegation;
    }

    private bool TriggerNegation()
    {
        int randomValue = GameManager.Instance.Rand.Next(1, 100);

        if (randomValue < NegateChance * 100) 
        {

            Shockwave();
            return true;
        }

        return false;
    }

    private void Shockwave()
    {
        Vector2 playerPosition = transform.position;
        float radius = 10f;

        RaycastHit2D[] Enemies = Physics2D.CircleCastAll(playerPosition, radius, Vector2.up);

        if (Enemies.Length > 0)
        {
            foreach (RaycastHit2D enemy in Enemies)
            {
                GameObject enemyObject = enemy.collider.gameObject;
                if (enemyObject.CompareTag("Enemy"))
                {
                    Vector2 EnemyPosition = enemyObject.transform.position;

                    Vector2 launchVector = (EnemyPosition - playerPosition).normalized;

                    EnemyScript enemyScript = enemyObject.GetComponent<EnemyScript>();

                    enemyScript.Launch(launchVector * 25f);

                }

            }
        }

    }

    protected override void Level2Effect()
    {
        NegateChance = 0.2f;
    }

    protected override void Level3Effect()
    {
        NegateChance = 0.25f;
    }

    protected override void Level4Effect()
    {
        NegateChance = 0.3f;
    }
    protected override void Level5Effect()
    {
        NegateChance = 0.35f;
    }

}
