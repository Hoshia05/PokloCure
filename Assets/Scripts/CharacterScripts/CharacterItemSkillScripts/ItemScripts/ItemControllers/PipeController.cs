using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeController : ItemController
{
    private float _hpHeal = 5f;
    private float _staminaHeal = 10f;

    // Start is called before the first frame update
    protected override void Launch()
    {
        base.Launch();
        ApplyHeal();
    }

    // Update is called once per frame
    private void ApplyHeal()
    {
        PlayerScript.Instance.HealHP(_hpHeal, false);
        PlayerScript.Instance.HealStamina(_staminaHeal);
    }

    protected override void Level2Effect()
    {
        _hpHeal = 6f; 
        _staminaHeal = 12f;
    }

    protected override void Level3Effect()
    {
        _hpHeal = 7f;
        _staminaHeal = 15f;
    }
    protected override void Level4Effect()
    {
        _hpHeal = 8f;
        _staminaHeal = 17f;
    }

    protected override void Level5Effect()
    {
        _hpHeal = 10f;
        _staminaHeal = 20f;
    }
}
