using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackUpgradeController : ItemController
{
    private float _attackBuffvalue = 0.05f;

    // Start is called before the first frame update
    void Start()
    {
        ApplyAtkBuff();
    }

    protected override void Update()
    {

    }

    protected override void LevelUpEffect()
    {
        ApplyAtkBuff();
    }


    void ApplyAtkBuff()
    {
        _buff.AttackMultiplierBuff = _attackBuffvalue;
        UpdateBuff();
    }

    // Update is called once per frame
    protected override void Level2Effect()
    {
        _attackBuffvalue = 0.10f;
    }

    protected override void Level3Effect()
    {
        _attackBuffvalue = 0.15f;
    }

    protected override void Level4Effect()
    {
        _attackBuffvalue = 0.2f;
    }

    protected override void Level5Effect()
    {
        _attackBuffvalue = 0.25f;
    }

    protected override void Level6Effect()
    {
        _attackBuffvalue = 0.3f;
    }

    protected override void Level7Effect()
    {
        _attackBuffvalue = 0.4f;
    }

}
