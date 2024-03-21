using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSlotController : ItemController
{
    // Start is called before the first frame update

    private void Start()
    {
        _currentLevel = PlayerScript.Instance.WeaponSlotCount;
        LevelUp();
    }

    protected override void LevelUpEffect()
    {
        PlayerScript.Instance.IncreaseWeaponSlot();
    }
}
