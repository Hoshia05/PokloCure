using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    protected float _damage;
    protected float _deathTime;
    protected int _pierce;
    protected float _speed;
    protected float _knockback;
    protected float _hitCooldown = 0;

    private bool _isHitCooldown;

    protected int _itemLevel;


    public void Awake()
    {
    }

    public void InitializeValue(float damage, float deathtime, int pierce, float speed, int level, float sizeScale, float knockback)
    {
        _damage = damage;
        _deathTime = deathtime;
        _pierce = pierce;
        _speed = speed;
        _itemLevel = level;
        _knockback = knockback;

        transform.localScale += new Vector3(sizeScale - 1f, sizeScale - 1f, 0);

        Destroy(gameObject, _deathTime);
    }

    public void SetHitCooldown(float hitcooldown)
    {
        _hitCooldown = hitcooldown;
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
            StartCoroutine(EnemyHitCoroutine(collision));
        }
    }

    private IEnumerator EnemyHitCoroutine(Collider2D collision)
    {
        GameObject enemy = collision.gameObject;
        EnemyScript script = enemy.GetComponent<EnemyScript>();

        //크리티컬 체크

        if (PlayerScript.Instance.CriticalCheck())
        {
            script.TakeDamage(PlayerScript.Instance.GetCritDamage(_damage),true);
        }
        else
        {
            script.TakeDamage(_damage);
        }

        if (_knockback != 0)
        {
            script.KnockbackEnemy(transform.position, _knockback);
        }

        CheckPierce();

        yield return new WaitForSeconds(_hitCooldown);
    }

}
