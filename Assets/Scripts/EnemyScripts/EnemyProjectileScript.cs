using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectileScript : MonoBehaviour
{
    private float _damage;

    private Rigidbody2D _rb;

    // Start is called before the first frame update

    public void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    public void InitializeProjectile(float damage)
    {
        _damage = damage;

        Destroy(gameObject, 8f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerScript playerScript = collision.gameObject.GetComponent<PlayerScript>();
            playerScript.TakeHit(_damage);

            Destroy(gameObject);
        }
    }
}
