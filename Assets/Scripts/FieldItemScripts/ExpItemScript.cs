using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpItemScript : FieldItemBase
{
    [SerializeField]
    private float _expValue = 0;

    public bool IsDebug;

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
