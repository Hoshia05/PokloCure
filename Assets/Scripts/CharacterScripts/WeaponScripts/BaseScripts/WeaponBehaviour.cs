using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    protected float _damage;
    protected float _deathTime;
    protected int _pierce;

    public void InitializeValue(float damage, float deathtime, int pierce)
    {
        _damage = damage;
        _deathTime = deathtime;
        _pierce = pierce;

        Destroy(gameObject, _deathTime);
    }

    private void CheckPierce()
    {
        _pierce--;

        if( _pierce <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            GameObject enemy = collision.gameObject;
            EnemyScript script = enemy.GetComponent<EnemyScript>();
            script.Damage(_damage);
            CheckPierce();
        }
    }
}
