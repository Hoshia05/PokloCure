using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpItemScript : MonoBehaviour
{
    private float _expValue = 10;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameObject player = collision.gameObject;
            PlayerMovement script = player.GetComponent<PlayerMovement>();
            script.GainEXP(_expValue);

            Destroy(gameObject);
        }
    }
}
