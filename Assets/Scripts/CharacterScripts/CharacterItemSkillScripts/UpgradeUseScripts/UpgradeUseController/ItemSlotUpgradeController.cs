using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSlotUpgradeController : ItemController
{
    // Start is called before the first frame update

    private void Start()
    {
        _currentLevel = PlayerScript.Instance.ItemSlotCount;
        LevelUp();
    }

    protected override void LevelUpEffect()
    {
        PlayerScript.Instance.IncreaseItemSlot();
    }
}
