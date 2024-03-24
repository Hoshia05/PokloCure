using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ExpItemScript : FieldItemBase
{
    [SerializeField]
    private float _expValue = 0;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameObject player = collision.gameObject;
            PlayerScript script = player.GetComponent<PlayerScript>();
            script.GainEXP(_expValue);

            Deactivate();
        }
    }

    public void Deactivate()
    {
        isPicked = false;
        gameObject.SetActive(false);
    }

    public void Activate(float expValue)
    {
        gameObject.SetActive(true);
        _expValue = expValue;
        gameObject.GetComponent<Collider2D>().enabled = true;
    }

}
