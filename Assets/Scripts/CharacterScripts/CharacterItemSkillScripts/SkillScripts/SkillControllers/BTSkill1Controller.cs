using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTSkill1Controller : ItemController
{
    private float buffValue = 0.01f;
    private float buffTime = 15f;

    protected override void Awake()
    {
        base.Awake();


        _maxStack = 15;

        StageManager.Instance.onEnemyKilled.AddListener(onKillEnemy);
        PlayerScript.Instance.onTakeDamage.AddListener(BuffPurge);
    }

    private void onKillEnemy()
    {
        if (_buffStack < _maxStack)
        {
            StartCoroutine(HunterBuff());
        }
    }

    private void onEatBurger()
    {
        if (_buffStack < _maxStack)
        {
            StartCoroutine(HunterBuff());
        }

    }

    IEnumerator HunterBuff()
    {
        _buffStack++;

        _buff.AttackMultiplierBuff = buffValue * _buffStack;

        UpdateBuff();

        yield return new WaitForSeconds(buffTime);

        if( _buffStack > 0)
            _buffStack--;
            

        _buff.AttackMultiplierBuff = buffValue * _buffStack;

        UpdateBuff();
    }


    private void BuffPurge()
    {
        _buffStack = 0;
        UpdateBuff();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        StageManager.Instance.onEnemyKilled.RemoveListener(onKillEnemy);
        PlayerScript.Instance.onTakeDamage.RemoveListener(BuffPurge);
    }


    protected override void Level2Effect()
    {
        buffValue = 0.012f;
    }

    protected override void Level3Effect()
    {
        buffValue = 0.015f;
    }
}
