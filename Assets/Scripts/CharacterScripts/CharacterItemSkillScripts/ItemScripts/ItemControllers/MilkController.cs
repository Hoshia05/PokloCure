using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MilkController : ItemController
{
    private float _attackSizeAndNockBackBuff = 0.10f;
    private float _itemEatDistance = 0.3f;

    protected void Start()
    {
        _buff.AttackSizeBuff = _attackSizeAndNockBackBuff;
        _buff.KnockBackBuff = _attackSizeAndNockBackBuff;
        _buff.EatDistanceMultiplier = _itemEatDistance;

        UpdateBuff();
    }

    protected override void LevelUpEffect()
    {
        _buff.AttackSizeBuff = _attackSizeAndNockBackBuff;
        _buff.KnockBackBuff = _attackSizeAndNockBackBuff;
        _buff.EatDistanceMultiplier = _itemEatDistance;

        UpdateBuff();
    }

    protected override void Level2Effect()
    {
        _attackSizeAndNockBackBuff = 0.15f;
        _itemEatDistance = 0.4f;
}
    protected override void Level3Effect()
    {
        _attackSizeAndNockBackBuff = 0.20f;
        _itemEatDistance = 0.5f;
    }
}
