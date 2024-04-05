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

        if(_enemyData == null)
            _enemyData = EnemyData;

        if (EnemyData == null && _enemyData == null)
            return;

        _collider.enabled = true;

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

        if (EnemyData.AnimatorController != null)
            _enemyAnim.runtimeAnimatorController = EnemyData.AnimatorController;

        if (EnemyData.BasicWeaponController != null)
            _weaponSlot = Instantiate(EnemyData.BasicWeaponController, _weaponSlot.transform);
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


        float degreeDif = 360 / _projectileNum;

        for (int i = 0; i < _projectileNum; i++)
        {
            float angle = i * degreeDif;
            Vector2 direction = Quaternion.Euler(0f, 0f, angle) * Vector2.up;

            GameObject projectile = Instantiate(RangedAttackProjectilePrefab, transform.position, Quaternion.identity);
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            EnemyProjectileScript epScript = projectile.GetComponent<EnemyProjectileScript>();
            epScript.InitializeProjectile(_rangedAttackDamage);
            rb.AddForce(direction * 300);

        }
    }

    protected void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameObject player = collision.gameObject;
            PlayerScript script = player.GetComponent<PlayerScript>();
            script.TakeHit(_currentBodyDamage);
        }
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameObject player = collision.gameObject;
            PlayerScript script = player.GetComponent<PlayerScript>();
            script.TakeHit(_currentBodyDamage);
        }
    }

    public void TakeDamage(float rawDamage, float knockBack, bool isCritical = false, string damageItem = null, float hitCooldown = 0)
    {

        if(DamageList.Contains(damageItem))
        {
            return;
        }
        else
        {

            float damage = DamageVariance(rawDamage);

            _currentMaxHP -= damage;

            //적 피격 모션 : 간단하게 한 0.3초 동안 빨갛게 되기
            StartCoroutine(HitAnimation());

            if(knockBack != 0)
                KnockbackEnemy(_playerPosition, knockBack);

            //위에 데미지 뜨는 애니메이션

            StageManager.Instance.GetDamageUIFromPool(transform.position, damage, isCritical);

            CheckDeath();

            if(hitCooldown > 0)
                StartCoroutine(DamageCoroutine(damageItem, hitCooldown));
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

        SpriteRenderer.color = Color.red;

        //GameObject HitEffect = Instantiate(GameManager.Instance.HitEffectPrefab, transform);
        StageManager.Instance.GetHitAnimationFromPool(transform.position);

        yield return new WaitForSeconds(0.4f);
        SpriteRenderer.color = Color.white;
    }

    public virtual void CheckDeath()
    {
        if (_currentMaxHP <= 0)
        {
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
        float dropChance = 5 + PlayerScript.Instance.BurgerDropChanceMultiplier;

        if(randval <= dropChance) 
        {
            Instantiate(GameManager.Instance.BurgerPrefab, RandomNearPosition(), Quaternion.identity);
        }
    }

    protected void DropDiamond()
    {
        int randval = GameManager.Instance.Rand.Next(1, 100);

        if (randval <= 33)
        {
            StageManager.Instance.GetMoneyItemFromPool(RandomNearPosition());
            //Instantiate(GameManager.Instance.DiamondPrefab, RandomNearPosition(), Quaternion.identity);
        }
    }

    protected float DamageVariance(float damage)
    {
        float minDamage = damage - 2;
        float maxDamage = damage + 2;

        float range = maxDamage - minDamage;
        double sample = GameManager.Instance.Rand.NextDouble();

        return ItemController.RoundValue((range * (float)sample) + minDamage);

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

        yield return new WaitForSeconds(stunTime);

        IsStunned = false;
    }

    public void BuffEnemy()
    {
        _buffMultiplier += 0.2f;
        InitializeWithSO(_enemyData);
    }
}
