using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiamondScript : FieldItemBase
{
    private int _coinValue;

    public bool IsDebug;

    private void Start()
    {
        _coinValue = 10;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StageManager.Instance.GainCoins(_coinValue);

            Destroy(gameObject);
        }
    }
}
