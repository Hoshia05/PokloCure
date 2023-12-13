using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PimanController : ItemController
{
    private float _hpGain = 15f;
    private float _perPointGain = 6f;

    private BuffObject _buff;

    private void Start()
    {
        PlayerScript.Instance.onMaxHPChange.AddListener(ApplyStats);

        _buff = new();


        ApplyStats();

    }

    protected override void LevelUpEffect()
    {
        ApplyStats();
    }

    private void ApplyStats()
    {
        _buff.HPBuff = _hpGain;
        _buff.AttackMultiplierBuff = CalculateAttackGain();

        PlayerScript.Instance.UpdateBuffDictionary(this, _buff);
    }


    private float CalculateAttackGain()
    {
        float CurrentMaxHP = PlayerScript.Instance.CurrentCharacterMaxHP;

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
