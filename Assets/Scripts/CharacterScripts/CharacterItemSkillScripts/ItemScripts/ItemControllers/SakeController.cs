using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SakeController : ItemController
{
    private float _buffCap = 0.1f;

    private const float EVERYSECOND = 1.0f;

    private bool _ateFood = false;

    void Start()
    {
        PlayerScript.Instance.onTakeDamage.AddListener(WhenHit);
        PlayerScript.Instance.onEatBurger.AddListener(WhenEat);

        _currentCooldownDuration = EVERYSECOND;
    }

    protected override void Launch()
    {
        base.Launch();

        _buff.CritMultiplierBuff += 0.01f;
        
        if(_buff.CritMultiplierBuff >= _buffCap)
        {
            _buff.CritMultiplierBuff = _buffCap;
        }

        UpdateBuff();
    }

    private void WhenHit()
    {
        _buff.CritMultiplierBuff = _buff.CritMultiplierBuff / 2;

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
        _buff.CritMultiplierBuff += 0.05f;
        UpdateBuff();

        yield return new WaitForSeconds(10f);


        _buff.CritMultiplierBuff -= 0.05f;
        UpdateBuff();
        _ateFood = false;

    }

    protected override void Level2Effect()
    {
        _buffCap = 0.15f;
    }

    protected override void Level3Effect()
    {
        _buffCap = 0.2f;
    }
}
