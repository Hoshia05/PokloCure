using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPillowController : ItemController
{
    private float _shieldGain = 15f;

    // Start is called before the first frame update
    void Start()
    {
        _buff.DamageReductionPercentage = 0.15f;

        UpdateBuff();

        InvokeRepeating("ReplenishShield", 0, 15f);
    }

    void ReplenishShield()
    {
        PlayerScript.Instance.SetShield(_shieldGain);
    }

    protected override void Level2Effect()
    {
        _shieldGain = 20f;
    }
    protected override void Level3Effect()
    {
        _shieldGain = 25f;
    }
    protected override void Level4Effect()
    {
        _shieldGain = 30f;
    }
    protected override void Level5Effect()
    {
        _shieldGain = 35f;
    }
}
