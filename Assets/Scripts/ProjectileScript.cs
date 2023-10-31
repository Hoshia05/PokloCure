using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    // Start is called before the first frame update
    private float _damage;

    void Start()
    {
        Destroy(gameObject, 3f);
        InitializeProjectile();
    }

    void InitializeProjectile()
    {
        _damage = 5.0f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            GameObject enemy = collision.gameObject;
            EnemyScript script = enemy.GetComponent<EnemyScript>();
            script.Damage(_damage);
        }
    }

}
