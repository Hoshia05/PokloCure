using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    protected int _itemLevel;

    protected Coroutine DeathCoroutine;

    protected bool _reachedPierceLimit;
    protected bool _weaponDestroyedAfterPierce;
    public bool WeaponDestroyedAfterPierce { set => _weaponDestroyedAfterPierce = value; }

    protected bool _cooldownWaitUntilProjectileDeath;
    public bool CooldownWaitUntilprojectileDeath { set => _cooldownWaitUntilProjectileDeath = value; }

    // 풀로 반환된 상태인지 (중복 반환 방지)
    private bool _released;

    public void InitializeValue(ItemController controller, float damage, float deathtime, int pierce, float speed, int level, float sizeScale, float knockback, float stunTime)
    {
        // 풀에서 재사용되므로 이전 사용 때의 상태를 전부 초기화한다
        _released = false;
        _reachedPierceLimit = false;
        _weaponDestroyedAfterPierce = false;
        _cooldownWaitUntilProjectileDeath = false;
        _hitCooldown = 0f;

        _controller = controller;
        _damage = damage;
        _deathTime = deathtime;
        _pierce = pierce;
        _speed = speed;
        _itemLevel = level;
        _knockback = knockback;
        _stunTime = stunTime;

        _itemName = controller.ItemData.ItemName;

        // 풀이 프리팹 크기로 되돌려 둔 상태에서 더하므로 재사용해도 누적되지 않는다
        transform.localScale += new Vector3(sizeScale - 1f, sizeScale - 1f, 0);

        DeathCoroutine = StartCoroutine(DestroyProjectile(_deathTime));

        OnSpawned();
    }

    // 꺼낼 때마다 호출된다. 예전에 Start() 에서 하던 1회성 초기화는 여기서 한다.
    protected virtual void OnSpawned()
    {
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
            DisengageWeapon();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if (_hitCooldown == 0)
        //    return;

        if (_reachedPierceLimit)
            return;

        if (collision.gameObject.CompareTag("Enemy"))
        {
            StartCoroutine(EnemyHitCoroutine(collision));
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (_reachedPierceLimit)
            return;

        if (_hitCooldown == 0)
            return;

        if (collision.gameObject.CompareTag("Enemy"))
        {
            StartCoroutine(EnemyHitCoroutine(collision));
        }
    }

    private IEnumerator EnemyHitCoroutine(Collider2D collision)
    {
        EnemyScript script = collision.gameObject.GetComponent<EnemyScript>();
        if (script == null)
            yield break;

        bool crit = PlayerScript.Instance.CriticalCheck();
        float dmg = crit ? PlayerScript.Instance.GetCritDamage(_damage) : _damage;

        // 히트 쿨다운 등으로 무시된 타격은 관통/기절을 소모하지 않는다
        bool hit = script.TakeDamage(dmg, _knockback, crit, _itemName, _hitCooldown);
        if (!hit)
            yield break;

        if (_stunTime > 0)
        {
            script.StunEnemy(_stunTime);
        }

        CheckPierce();
    }

    public void DestroyProjectileImmediately()
    {
        _controller.CurrentProjectiles.Remove(this);
        EndProcess();
    }

    protected IEnumerator DestroyProjectile(float time = 0f)
    {
        yield return new WaitForSeconds(time);

        _controller.CurrentProjectiles.Remove(this);
        EndProcess();
    }

    // 컨트롤러가 목록을 직접 비우는 경로에서 쓴다 (목록을 순회 중에 수정하지 않도록 여기서는 지우지 않는다)
    public void DestroyProjectilesNow()
    {
        ReleaseToPool(false);
    }

    protected void DisengageWeapon()
    {
        _reachedPierceLimit = true;
        if (_weaponDestroyedAfterPierce)
            EndProcess();
    }

    public void ResetCooldown()
    {
        if(_controller != null)
            _controller.ResetCooldown();
    }

    public void EndProcess()
    {
        if (_released)
            return;

        if(_cooldownWaitUntilProjectileDeath)
            ResetCooldown();

        ReleaseToPool(true);
    }

    private void ReleaseToPool(bool removeFromController)
    {
        if (_released)
            return;

        _released = true;

        // 풀로 돌아간 뒤에 다른 컨트롤러가 다시 쓸 수 있으므로 옛 컨트롤러 목록에서 반드시 빼 둔다
        if (removeFromController && _controller != null)
            _controller.CurrentProjectiles.Remove(this);

        // 풀에서 온 오브젝트면 풀로 반환하고 (비활성화되면서 코루틴도 모두 멈춘다), 아니면 예전처럼 파괴한다
        if (TryGetComponent(out PooledObject pooled))
            pooled.Release();
        else
            Destroy(gameObject);
    }

}
