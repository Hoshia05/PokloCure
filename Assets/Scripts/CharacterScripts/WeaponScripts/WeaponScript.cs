using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponScript : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    protected float _damage;
    [SerializeField]
    protected float _deathTime;

    void Start()
    {
        Destroy(gameObject, _deathTime);
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
