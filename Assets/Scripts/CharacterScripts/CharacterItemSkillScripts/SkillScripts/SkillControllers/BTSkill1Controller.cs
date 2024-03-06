using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTSkill1Controller : ItemController
{
    private float buffValue = 0.01f;
    private float buffTime = 15f;

    private int _maxStack = 15;
    private int _currentStack = 0;

    protected override void Awake()
    {
        base.Awake();

        StageManager.Instance.onEnemyKilled.AddListener(onKillEnemy);
        PlayerScript.Instance.onTakeDamage.AddListener(BuffPurge);
    }

    private void onKillEnemy()
    {
        if (_currentStack < _maxStack)
        {
            StartCoroutine(HunterBuff());
        }
    }

    private void onEatBurger()
    {
        if (_currentStack < _maxStack)
        {
            StartCoroutine(HunterBuff());
        }

    }

    IEnumerator HunterBuff()
    {
        _currentStack++;

        _buff.AttackMultiplierBuff = buffValue * _currentStack;

        PlayerScript.Instance.UpdateBuffDictionary(this, _buff);

        yield return new WaitForSeconds(buffTime);

        if( _currentStack > 0)
            _currentStack--;
            

        _buff.AttackMultiplierBuff = buffValue * _currentStack;

        PlayerScript.Instance.UpdateBuffDictionary(this, _buff);
    }


    private void BuffPurge()
    {
        _currentStack = 0;
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
