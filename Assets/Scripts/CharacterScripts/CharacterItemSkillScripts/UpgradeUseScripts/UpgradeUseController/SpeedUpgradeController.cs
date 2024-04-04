using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedUpgradeController : ItemController
{
    private float _speedBuffvalue = 0.05f;

    // Start is called before the first frame update
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
        _buff.SpeedMultiplierBuff = _speedBuffvalue;
        UpdateBuff();
    }

    // Update is called once per frame
    protected override void Level2Effect()
    {
        _speedBuffvalue = 0.10f;
    }

    protected override void Level3Effect()
    {
        _speedBuffvalue = 0.15f;
    }

    protected override void Level4Effect()
    {
        _speedBuffvalue = 0.2f;
    }

    protected override void Level5Effect()
    {
        _speedBuffvalue = 0.25f;
    }

    protected override void Level6Effect()
    {
        _speedBuffvalue = 0.3f;
    }

    protected override void Level7Effect()
    {
        _speedBuffvalue = 0.4f;
    }
}
