using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPUpgradeController : ItemController
{
    private float Buffvalue = 10f;

    void Start()
    {
        ApplyBuff();
    }


    protected override void LevelUpEffect()
    {
        ApplyBuff();
    }


    void ApplyBuff()
    {
        _buff.HPBuff = Buffvalue;
        UpdateBuff();
    }

    // Update is called once per frame
    protected override void Level2Effect()
    {
        Buffvalue = 20f;
    }

    protected override void Level3Effect()
    {
        Buffvalue = 30f;
    }

    protected override void Level4Effect()
    {
        Buffvalue = 40f;
    }

    protected override void Level5Effect()
    {
        Buffvalue = 50f;
    }

    protected override void Level6Effect()
    {
        Buffvalue = 70f;
    }

    protected override void Level7Effect()
    {
        Buffvalue = 100f;
    }
}
