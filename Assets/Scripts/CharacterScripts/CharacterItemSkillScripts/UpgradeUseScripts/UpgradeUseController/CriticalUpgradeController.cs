using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CriticalUpgradeController : ItemController
{

    private float Buffvalue = 0.05f;

    void Start()
    {
        ApplyBuff();
    }

    protected override void Update()
    {

    }

    protected override void LevelUpEffect()
    {
        ApplyBuff();
    }


    void ApplyBuff()
    {
        _buff.CritMultiplierBuff = Buffvalue;
        _buff.CritDamageBuff = Buffvalue;
        UpdateBuff();
    }

    // Update is called once per frame
    protected override void Level2Effect()
    {
        Buffvalue = 0.10f;
    }

    protected override void Level3Effect()
    {
        Buffvalue = 0.15f;
    }

    protected override void Level4Effect()
    {
        Buffvalue = 0.2f;
    }

    protected override void Level5Effect()
    {
        Buffvalue = 0.25f;
    }

    protected override void Level6Effect()
    {
        Buffvalue = 0.3f;
    }

    protected override void Level7Effect()
    {
        Buffvalue = 0.4f;
    }
}
