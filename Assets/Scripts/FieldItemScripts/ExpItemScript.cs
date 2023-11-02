using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpItemScript : FieldItemBase
{
    private float _expValue;

    private void Start()
    {
        _expValue = Random.value * 10;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameObject player = collision.gameObject;
            PlayerScript script = player.GetComponent<PlayerScript>();
            script.GainEXP(_expValue);

            Destroy(gameObject);
        }
    }
}