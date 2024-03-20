using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GTSkill1Controller : ItemController
{
    private int _additionalProjectile = 1;
    private float _rangedCooldownBuff = 0.05f;

    private void Start()
    {
        ApplyBuff();
    }
    protected override void LevelUpEffect()
    {
        ApplyBuff();
    }

    void ApplyBuff()
    {
        _buff.RangedCooldownBuff = _rangedCooldownBuff;
        _buff.RangedProjectileBuff = _additionalProjectile;
        UpdateBuff();
    }

    protected override void Level2Effect()
    {
        _additionalProjectile = 2;
        _rangedCooldownBuff = 0.1f;
    }

    protected override void Level3Effect()
    {
        _additionalProjectile = 3;
        _rangedCooldownBuff = 0.12f;
    }
}
