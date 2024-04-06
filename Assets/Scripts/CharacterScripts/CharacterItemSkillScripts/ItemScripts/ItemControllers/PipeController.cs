using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeController : ItemController
{
    private float _speedBuff = 0.15f;
    private float _hasteBuff = 0.1f;

    protected override void Launch()
    {
        ApplyBuff();
    }
    protected override void LevelUpEffect()
    {
        ApplyBuff();
    }

    void ApplyBuff()
    {
        _buff.SpeedMultiplierBuff = _speedBuff;
        _buff.HasteMultiplierBuff = _hasteBuff;
        UpdateBuff();
    }

    protected override void Level2Effect()
    {
        _speedBuff = 0.3f;
        _hasteBuff = 0.2f;
    }

    protected override void Level3Effect()
    {
        _speedBuff = 0.45f;
        _hasteBuff = 0.3f;
    }
}
