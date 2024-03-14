using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SakeController : ItemController
{

    private float buffValue = 0.01f;

    private const float EVERYSECOND = 1.0f;

    private bool _ateFood = false;

    void Start()
    {

        _maxStack = 10;


        PlayerScript.Instance.onTakeDamage.AddListener(WhenHit);
        PlayerScript.Instance.onEatBurger.AddListener(WhenEat);

        _currentCooldownDuration = EVERYSECOND;
    }

    protected override void Launch()
    {
        base.Launch();

        _buffStack++;

        _buff.CritMultiplierBuff = _buffStack * buffValue;
        
        if(_buffStack >= _maxStack)
        {
            _buffStack = _maxStack;
        }

        UpdateBuff();
    }

    private void WhenHit()
    {
        _buffStack = _buffStack / 2;

        UpdateBuff();
    }

    private void WhenEat()
    {
        if (!_ateFood)
        {
            StartCoroutine(EatBuff());
        }
    }

    private IEnumerator EatBuff()
    {
        _ateFood = true;
        _buffStack += 5;
        UpdateBuff();

        yield return new WaitForSeconds(10f);


        _buffStack -= 5;
        if( _buffStack < 0 )
        {
            _buffStack = 0;
        }
        UpdateBuff();
        _ateFood = false;

    }

    protected override void Level2Effect()
    {
        _maxStack = 15;
    }

    protected override void Level3Effect()
    {
        _maxStack = 20;
    }
}
