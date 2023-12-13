using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpItemScript : FieldItemBase
{
    private float _expValue;

    public bool IsDebug;

    private void Start()
    {
        _expValue = Random.value * 10;
        if (IsDebug)
            _expValue *= 10;
    }

    public void SetExpValue(float expValue)
    {
        _expValue = expValue;
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
