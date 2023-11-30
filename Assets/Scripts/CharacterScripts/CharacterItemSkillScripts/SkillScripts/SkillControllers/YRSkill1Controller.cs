using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YRSkill1Controller : ItemController
{
    private float buffValue = 0.05f;
    private float buffTime = 5f;

    private int _maxStack = 3;
    private int _currentStack = 0;

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

    protected override void OnDestroy()
    {
        base.OnDestroy();
        PlayerScript.Instance.onEatBurger.RemoveListener(onEatBurger);
    }


    protected override void Level2Effect()
    {
        buffValue = 0.07f;
    }

    protected override void Level3Effect()
    {
        buffValue = 0.1f;
    }
}
