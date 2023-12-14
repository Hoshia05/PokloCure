using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PimanController : ItemController
{
    private float _hpGain = 15f;
    private float _perPointGain = 6f;

    private float _previousMaxHP = 0;

    private void Start()
    {
        PlayerScript.Instance.onMaxHPChange.AddListener(ApplyAttackBuff);

        ApplyHPBuff();

    }

    protected override void LevelUpEffect()
    {
        ApplyHPBuff();
    }

    private void ApplyHPBuff()
    {
        _buff.HPBuff = _hpGain;
        UpdateBuff();
    }

    private void ApplyAttackBuff(float newMaxHP)
    {
        if (newMaxHP == _previousMaxHP)
            return;

        _previousMaxHP = newMaxHP;

        _buff.AttackMultiplierBuff = CalculateAttackGain(newMaxHP);

        UpdateBuff();

    }


    private float CalculateAttackGain(float CurrentMaxHP)
    {
        int ATKIncrementValuePercentage = (int)(CurrentMaxHP / _perPointGain);

        return ATKIncrementValuePercentage / 100f;
    }

    protected override void Level2Effect()
    {
        _hpGain = 20f;
        _perPointGain = 5f;
    }

    protected override void Level3Effect()
    {
        _hpGain = 25f;
        _perPointGain = 4f;
    }
}
