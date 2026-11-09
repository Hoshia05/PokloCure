using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YRSkill1Controller : ItemController
{
    private float buffValue = 0.05f;
    private float buffTime = 15f;

    private float _launchForce = 10f;

    protected override void Awake()
    {
        base.Awake();

        _maxStack = 3;

        PlayerScript.Instance.onEatBurger.AddListener(onEatBurger);
    }

    private void onEatBurger()
    {
        if(_buffStack < _maxStack)
        {
            StartCoroutine(BurgerBuff());
        }

        BurgerShockwave();
    }

    IEnumerator BurgerBuff()
    {
        _buffStack++;

        _buff.AttackMultiplierBuff = buffValue * _buffStack;

        UpdateBuff();

        yield return new WaitForSeconds(buffTime);

        _buffStack--;

        _buff.AttackMultiplierBuff = buffValue * _buffStack;

        UpdateBuff();
    }

    private void BurgerShockwave()
    {
        Vector2 playerPosition = transform.position;
        float radius = 10f;

        // CircleCastAll은 거리 기본값이 무한대라 위쪽 전체를 훑는다. 반경 안만 검사한다.
        Collider2D[] Enemies = Physics2D.OverlapCircleAll(playerPosition, radius);

        foreach (Collider2D enemy in Enemies)
        {
            GameObject enemyObject = enemy.gameObject;
            if (enemyObject.CompareTag("Enemy"))
            {
                Vector2 EnemyPosition = enemyObject.transform.position;

                Vector2 launchVector = (EnemyPosition - playerPosition).normalized;

                EnemyScript enemyScript = enemyObject.GetComponent<EnemyScript>();

                enemyScript.Launch(launchVector * _launchForce);
            }
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (PlayerScript.Instance != null)
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
        _launchForce = 40f;
    }
}
