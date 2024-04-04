using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GTGunBehaviour : ItemBehaviour
{
    private void OnEnable()
    {
        _cooldownWaitUntilProjectileDeath = false;
    }
}
