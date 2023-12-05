using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YRSkill1Controller : ItemController
{
    private float buffValue = 0.05f;
    private float buffTime = 5f;

    private int _maxStack = 3;
    private int _currentStack = 0;

    private float _launchForce = 10f;

    protected override void Awake()
    {
        base.Awake();

        PlayerScript.Instance.onEatBurger.AddListener(onEatBurger);
    }

    private void onEatBurger()
    {
        if(_currentStack < _maxStack)
        {
            StartCoroutine(BurgerBuff());
        }

        BurgerShockwave();
    }

    IEnumerator BurgerBuff()
    {
        _currentStack++;
        float currentBuffValue = buffValue;
        PlayerScript.Instance.AttackStatChange(currentBuffValue);

        yield return new WaitForSeconds(buffTime);

        PlayerScript.Instance.AttackStatChange(-currentBuffValue);
        _currentStack--;
    }

    private void BurgerShockwave()
    {
        Vector2 playerPosition = transform.position;
        float radius = 10f;

        RaycastHit2D[] Enemies = Physics2D.CircleCastAll(playerPosition, radius, Vector2.up);

        if(Enemies.Length > 0 )
        {
            foreach(RaycastHit2D enemy in Enemies)
            {
                GameObject enemyObject = enemy.collider.gameObject;
                if (enemyObject.CompareTag("Enemy"))
                {
                    Vector2 EnemyPosition = enemyObject.transform.position;

                    Vector2 launchVector = (EnemyPosition - playerPosition).normalized;
                    
                    EnemyScript enemyScript = enemyObject.GetComponent<EnemyScript>();

                    enemyScript.Launch(launchVector * _launchForce);

                }

            }
        }

    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        PlayerScript.Instance.onEatBurger.RemoveListener(onEatBurger);
    }


    protected override void Level2Effect()
    {
        buffValue = 0.07f;
        _launchForce = 25f;
    }

    protected override void Level3Effect()
    {
        buffValue = 0.1f;
        _launchForce = 50f;
    }
}
