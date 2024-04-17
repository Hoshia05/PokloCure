using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SakeController : ItemController
{

    private float buffValue = 0.01f;

    private const float EVERYSECOND = 1.0f;

    private bool _ateFood;
    private bool _isDrunk;
    private bool _finalLevel;

    private Coroutine _drunkCoroutine;

    void Start()
    {

        _maxStack = 10;


        PlayerScript.Instance.onTakeDamage.AddListener(WhenHit);
        PlayerScript.Instance.onEatBurger.AddListener(WhenEat);

        _currentCooldownDuration = EVERYSECOND;
    }

    protected override void Update()
    {
        base.Update();

        if (_buffStack == _maxStack && !_isDrunk && _finalLevel)
        {
            _drunkCoroutine = StartCoroutine(DrunkEffect());
        }
    }

    protected override void Launch()
    {
        base.Launch();

        _buffStack++;
        ApplyBuff();
    }

    void ApplyBuff()
    {

        _buff.CritMultiplierBuff = _buffStack * buffValue;
        _buff.AttackMultiplierBuff = _buffStack * buffValue;

        if (_buffStack >= _maxStack)
        {
            _buffStack = _maxStack;
        }

        UpdateBuff();
    }

    private void WhenHit()
    {
        _buffStack = _buffStack / 2;
        BreakDrunk();

        ApplyBuff();
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

    IEnumerator DrunkEffect()
    {
        _isDrunk = true;

        float waitTime = GameManager.Instance.Rand.Next(1, 5);
        float drunkTime = GameManager.Instance.Rand.Next(1, 5);

        yield return new WaitForSeconds(waitTime);

        PlayerControl.Instance.IsDrunk = true;

        yield return new WaitForSeconds(drunkTime);

        PlayerControl.Instance.IsDrunk = false;

        _isDrunk = false;
    }

    void BreakDrunk()
    {
        _isDrunk = false;
        PlayerControl.Instance.IsDrunk = false;
        
        if(_drunkCoroutine != null)
            StopCoroutine(_drunkCoroutine);
    }


    protected override void Level2Effect()
    {
        _maxStack = 15;
    }

    protected override void Level3Effect()
    {
        _maxStack = 20;
        _finalLevel = true;
    }
}
