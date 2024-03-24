using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class ItemBehaviour : MonoBehaviour
{
    protected ItemController _controller;

    protected string _itemName;
    protected float _damage;
    protected float _deathTime;
    protected int _pierce;
    protected float _speed;
    protected float _knockback;
    protected float _hitCooldown = 0;
    protected float _stunTime = 0f;

    private bool _isHitCooldown;

    protected int _itemLevel;

    protected Coroutine DeathCoroutine;


    public void InitializeValue(ItemController controller, float damage, float deathtime, int pierce, float speed, int level, float sizeScale, float knockback, float stunTime)
    {
        _controller = controller;
        _damage = damage;
        _deathTime = deathtime;
        _pierce = pierce;
        _speed = speed;
        _itemLevel = level;
        _knockback = knockback;
        _stunTime = stunTime;

        _itemName = controller.ItemData.ItemName;

        transform.localScale += new Vector3(sizeScale - 1f, sizeScale - 1f, 0);

        DeathCoroutine = StartCoroutine(DestroyProjectile(_deathTime));
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
            ResetCooldown();
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if (_hitCooldown == 0)
        //    return;

        if (collision.gameObject.CompareTag("Enemy"))
        {
            StartCoroutine(EnemyHitCoroutine(collision));
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (_hitCooldown == 0)
            return;

        if (collision.gameObject.CompareTag("Enemy"))
        {
            StartCoroutine(EnemyHitCoroutine(collision));
        }
    }

    private IEnumerator EnemyHitCoroutine(Collider2D collision)
    {
        GameObject enemy = collision.gameObject;
        EnemyScript script = enemy.GetComponent<EnemyScript>();


        if (PlayerScript.Instance.CriticalCheck())
        {
            script.TakeDamage(PlayerScript.Instance.GetCritDamage(_damage), _knockback, true, _itemName, _hitCooldown);
        }
        else
        {
            script.TakeDamage(_damage, _knockback, false, _itemName, _hitCooldown);
        }

        if(_stunTime > 0)
        {
            script.StunEnemy(_stunTime);
        }

        CheckPierce();

        yield return new WaitForSeconds(_hitCooldown);
    }

    protected IEnumerator DestroyProjectile(float time = 0f)
    {
        yield return new WaitForSeconds(time);

        _controller.CurrentProjectiles.Remove(this);
        Destroy(gameObject);
    }

    public void DestroyProjectilesNow()
    {
        Destroy(gameObject);
    }

    public void ResetCooldown()
    {
        _controller.ResetCooldown();
    }

}
