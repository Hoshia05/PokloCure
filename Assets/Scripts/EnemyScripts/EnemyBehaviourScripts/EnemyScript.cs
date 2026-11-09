using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    // Start is called before the first frame update

    protected GameObject _playerCharacter;
    protected Vector2 _playerPosition;
    protected Rigidbody2D _rb;
    protected Collider2D _collider;
    protected EnemyFSM _enemyFSM;

    protected Vector2 _enemyLineOfSight;

    [SerializeField]
    protected EnemyBase _enemyData;

    public EnemyClass EClass;
    

    [Header("캐릭스펙")]
    protected float _currentMovementSpeed;
    public float CurrentMovementSpeed { get => _currentMovementSpeed; }
    protected float _currentAttackCoolTime;
    protected float _currentMaxHP;
    protected float _currentBodyDamage;
    protected float _currentCriticalChance;
    protected float _dropExpValue;
    protected EnemyType _enemyType;
    protected bool _isBoss;
    protected bool _isDying;
    protected float _currentHP;
    protected bool _initialized;
    protected GameObject _weaponInstance;

    public const float BuffStep = 0.35f;
    protected float _buffMultiplier = 1f;

    //Pattern Spawn 관련
    public bool IsPatternSpawn;
    public bool IsStunned;

    //공격패턴 관련
    private GameObject RangedAttackProjectilePrefab;
    private float _projectileNum;

    private float _rangedAttackDamage;
    private float _rangedAttackDistance;
    public float RangedAttackDistance { get => _rangedAttackDistance; }

    private float _rangedAttackCooltime;
    public float RangedAttackCooltime { get => _rangedAttackCooltime; }

    public Vector2 FreezePosition;


    [Header("무기슬롯관련")]
    [SerializeField]
    protected GameObject _weaponSlot;

    [SerializeField]
    public SpriteRenderer SpriteRenderer;

    protected Animator _enemyAnim; //Not now...

    List<String> DamageList;

    [Header("사운드관련")]

    [SerializeField]
    private AudioClip _damageSoundClip;


    protected void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
        _enemyFSM = GetComponent<EnemyFSM>();
        DamageList = new();
    }

    protected void Start()
    {
        _playerCharacter = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    protected void Update()
    {
        _playerPosition = _playerCharacter != null ? _playerCharacter.transform.position : transform.position;
        //if(!IsPatternSpawn && !IsStunned)
        //    Movement();
        UpdateLOS();
    }

    public void InitializeWithSO(EnemyBase EnemyData)
    {
        if (EnemyData == null)
            EnemyData = _enemyData;

        if (EnemyData == null)
            return;

        // 1회성 초기화: 데이터 보관, 애니메이터 캐시, 무기 생성
        if (!_initialized)
        {
            if (_enemyData == null)
                _enemyData = EnemyData;

            _enemyAnim = GetComponent<Animator>();

            if (_enemyData.BasicWeaponController != null && _weaponSlot != null)
                _weaponInstance = Instantiate(_enemyData.BasicWeaponController, _weaponSlot.transform);

            _initialized = true;
        }

        ApplyStats();
        _currentHP = _currentMaxHP;
    }

    // 버프 배율에 의존하는 수치만 적용한다. 현재 HP를 건드리거나 오브젝트를 생성하지 않는다.
    protected void ApplyStats()
    {
        EnemyBase EnemyData = _enemyData;

        EClass = _enemyData.EnemyClass;

        //캐릭터 특정 수치들
        _currentMovementSpeed = EnemyData.SpeedMultiplier * EnemyBase._baseSpeed * _buffMultiplier;
        _currentMaxHP = EnemyData.HP * _buffMultiplier;
        _currentBodyDamage = EnemyData.BodyDamage * _buffMultiplier;
        _currentCriticalChance = EnemyData.CriticalChance;
        _dropExpValue = EnemyData.DropEXP * _buffMultiplier;
        _isBoss = EnemyData.isBossEnemy;

        if (EnemyData.EnemyType == EnemyType.RANGED)
        {
            //원거리 공격 관련
            RangedAttackProjectilePrefab = _enemyData.RangedAttackProjectilePrefab;
            _projectileNum = _enemyData._projectileNum;
            _rangedAttackDamage = _enemyData._rangedAttackDamage;
            _rangedAttackDistance = _enemyData.RangedAttackDistance;
            _rangedAttackCooltime = _enemyData.RangedAttackCooltime;

        }


        SpriteRenderer.sprite = EnemyData.EnemySprite;

        transform.localScale = new Vector3(EnemyData.SizeScale, EnemyData.SizeScale, EnemyData.SizeScale);

        if (EnemyData.AnimatorController != null && _enemyAnim != null)
            _enemyAnim.runtimeAnimatorController = EnemyData.AnimatorController;
    }

    // 풀에서 꺼내 재사용할 때 이전 생애의 상태(기절, 색, 코루틴 등)를 초기화한다.
    // 반드시 오브젝트가 활성화된 뒤에 호출할 것 (코루틴 정리).
    public void ResetForSpawn()
    {
        StopAllCoroutines();
        _isDying = false;
        IsStunned = false;
        IsPatternSpawn = false;
        DamageList.Clear();
        SpriteRenderer.color = Color.white;
        SpriteRenderer.enabled = true;
        _collider.enabled = true;
        _nextContactHitTime = 0f;
        // KillEnemy가 속도를 0으로 만들기 때문에, 재사용 시 스탯(이동속도 등)을 다시 적용한다.
        if (_initialized)
            ApplyStats();
        _currentHP = _currentMaxHP;
    }

    public void SetForPatternSpawn(SpawnType type)
    {
        IsPatternSpawn = true;

        Vector2 InitialTargetPosition = _playerCharacter.transform.position;

        switch(type)
        {
            case SpawnType.HORDE:
                break;
            case SpawnType.RING:
                break;
        }

    }


    protected void Movement()
    {
        float step = _currentMovementSpeed * Time.deltaTime;
        transform.position = Vector2.MoveTowards(transform.position, _playerPosition, step);

    }

    protected void UpdateLOS()
    {
        _enemyLineOfSight = (_playerPosition - (Vector2)transform.position).normalized;

        SpriteRenderer.flipX = _enemyLineOfSight.x > 0 ? false : true;
    }

    public void ConductRangedAttack()
    {
        if(_projectileNum == 0) return;

        float startOffset = GameManager.Instance.Rand.Next(30);

        float degreeDif = 360 / _projectileNum;

        Vector2 direction = _enemyLineOfSight;

        GameObject projectile = Instantiate(RangedAttackProjectilePrefab, transform.position, Quaternion.identity);
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        EnemyProjectileScript epScript = projectile.GetComponent<EnemyProjectileScript>();
        epScript.InitializeProjectile(_rangedAttackDamage);
        rb.AddForce(direction * 300);
        rb.AddTorque(500f);
    }

    // 플레이어와 닿아 있는 동안 ContactHitCooldown 간격으로 계속 피해를 준다.
    // (Enter에서만 때리면, 피격 무적이 끝난 뒤에도 붙어 있는 적이 더 이상 피해를 주지 못한다.)
    private const float ContactHitCooldown = 0.5f;
    private float _nextContactHitTime;

    private void TryContactHit(GameObject other)
    {
        if (!other.CompareTag("Player") || Time.time < _nextContactHitTime)
            return;

        PlayerScript script = other.GetComponent<PlayerScript>();
        if (script == null)
            return;

        _nextContactHitTime = Time.time + ContactHitCooldown;
        script.TakeHit(_currentBodyDamage);
    }

    protected void OnCollisionEnter2D(Collision2D collision)
    {
        TryContactHit(collision.gameObject);
    }

    protected void OnCollisionStay2D(Collision2D collision)
    {
        TryContactHit(collision.gameObject);
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        TryContactHit(collision.gameObject);
    }

    protected void OnTriggerStay2D(Collider2D collision)
    {
        TryContactHit(collision.gameObject);
    }

    // 반환값: 실제로 피해가 들어갔으면 true, 무시되었으면(사망 중 / 히트 쿨다운) false
    public bool TakeDamage(float rawDamage, float knockBack, bool isCritical = false, string damageItem = null, float hitCooldown = 0)
    {
        if (_isDying)
            return false;

        if(DamageList.Contains(damageItem))
        {
            return false;
        }
        else
        {

            float damage = DamageVariance(rawDamage);

            _currentHP -= damage;

            //적 피격 모션 : 간단하게 한 0.3초 동안 빨갛게 되기
            StartCoroutine(HitAnimation());

            if(knockBack != 0)
                KnockbackEnemy(_playerPosition, knockBack);

            //위에 데미지 뜨는 애니메이션

            StageManager.Instance.GetDamageUIFromPool(transform.position, damage, isCritical);

            CheckDeath();

            if(hitCooldown > 0)
                StartCoroutine(DamageCoroutine(damageItem, hitCooldown));

            return true;
        }

    }

    private IEnumerator DamageCoroutine(string damageItem, float hitCooldown)
    {
        DamageList.Add(damageItem);

        yield return new WaitForSeconds(hitCooldown);

        DamageList.Remove(damageItem);
    }

    protected IEnumerator HitAnimation()
    {
        SoundFXManager.Instance.PlaySoundFXClip(_damageSoundClip, transform, 0.3f);

        SpriteRenderer.color = IsStunned ? Color.blue : Color.red;

        //GameObject HitEffect = Instantiate(GameManager.Instance.HitEffectPrefab, transform);
        StageManager.Instance.GetHitAnimationFromPool(transform.position);

        yield return new WaitForSeconds(0.4f);
        SpriteRenderer.color = Color.white;
    }

    public virtual void CheckDeath()
    {
        if (_currentHP <= 0 && !_isDying)
        {
            _isDying = true;
            StartCoroutine(KillEnemy());
        }
    }
    
    protected IEnumerator KillEnemy()
    {
        StageManager.Instance.UpdateKill(_enemyData.EnemyScore);
        Collider2D coll = GetComponent<Collider2D>();
        coll.enabled = false;

        yield return new WaitForSeconds(0.1f);

        _currentMovementSpeed = 0;
        Color originalColor = SpriteRenderer.color;

        float elapsedTime = 0f;

        while (elapsedTime < 0.5f)
        {
            float alpha = Mathf.Lerp(originalColor.a, 0f, elapsedTime / 0.5f);

            SpriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        SpriteRenderer.color = Color.white;

        //경험치템 소환
        StageManager.Instance.GetEXPItemFromPool(transform.position, _dropExpValue);

        //버거소환
        DropBurger();
        //다이아소환
        DropDiamond();

        if(_enemyData.EnemyClass == EnemyClass.ELITE && GameManager.Instance.RollRandom(5f))
        {
            Instantiate(GameManager.Instance.TreasureBoxPrefab, RandomNearPosition(), Quaternion.identity);
        }


        //Destroy(gameObject);
        DamageList = new();
        StopAllCoroutines();
        StageManager.Instance.EnemyDeathEvent(gameObject);
        //StageManager.Instance.CurrentEnemyCount--;

    }

    protected Vector2 RandomNearPosition()
    {
        return new Vector2(transform.position.x + GameManager.Instance.Rand.Next(1, 10) * 0.1f, transform.position.y + GameManager.Instance.Rand.Next(1, 10) * 0.1f);
    }

    protected void DropBurger()
    {
        int randval = GameManager.Instance.Rand.Next(1, 100);
        float dropChance = 1 + PlayerScript.Instance.BurgerDropChanceMultiplier;

        if(randval <= dropChance) 
        {
            Instantiate(GameManager.Instance.SteakPrefab, RandomNearPosition(), Quaternion.identity);
        }
    }

    protected void DropDiamond()
    {
        int randval = GameManager.Instance.Rand.Next(1, 100);

        if (randval <= 33)
        {
            StageManager.Instance.GetMoneyItemFromPool(RandomNearPosition());
        }
    }

    protected float DamageVariance(float damage)
    {
        float minDamage = damage - 2;
        float maxDamage = damage + 2;

        float range = maxDamage - minDamage;
        double sample = GameManager.Instance.Rand.NextDouble();

        return Mathf.Max(1f, ItemController.RoundValue((range * (float)sample) + minDamage));

    }

    public void KnockbackEnemy(Vector2 playerPosition, float LaunchForce)
    {
        Vector2 EnemyPosition = transform.position;

        Vector2 launchVector = (EnemyPosition - playerPosition).normalized;

        Launch(launchVector * LaunchForce);
    }

    public void Launch(Vector2 LaunchVector)
    {
        StartCoroutine(LaunchCoroutine(LaunchVector));
    }

    protected IEnumerator LaunchCoroutine(Vector2 LaunchVector)
    {
        float elapsedTime = 0f;
        Vector3 initialPosition = transform.position;
        Vector3 targetPosition = initialPosition + (Vector3)(LaunchVector);

        while (elapsedTime < 0.15f)
        {
            transform.position = Vector3.Lerp(initialPosition, targetPosition, elapsedTime / 0.15f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the enemy reaches the exact target position
        transform.position = targetPosition;
    }

    public void StunEnemy(float stunTime)
    {
        StartCoroutine(StunCoroutine(stunTime));
    }

    IEnumerator StunCoroutine(float stunTime)
    {
        IsStunned = true;
        FreezePosition = transform.position;

        yield return new WaitForSeconds(stunTime);

        IsStunned = false;
    }

    // 풀에서 새로 만든 적이 현재 강화 단계를 바로 따라가도록 배율을 지정한다.
    public void SetBuffMultiplier(float multiplier)
    {
        _buffMultiplier = multiplier;

        if (_initialized)
        {
            ApplyStats();
            _currentHP = _currentMaxHP;
        }
    }

    public void BuffEnemy()
    {
        if (!_initialized)
            return;

        float oldMax = _currentMaxHP;
        _buffMultiplier += BuffStep;
        ApplyStats();

        if (gameObject.activeInHierarchy && oldMax > 0)
            _currentHP *= _currentMaxHP / oldMax;   // 현재 HP 비율 유지
        else
            _currentHP = _currentMaxHP;
    }
}
