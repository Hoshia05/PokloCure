using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class EnemyScript : MonoBehaviour
{
    // Start is called before the first frame update

    protected GameObject _playerCharacter;
    protected Vector2 _playerPosition;
    protected Rigidbody2D _rb;
    protected Collider2D _collider;

    protected Vector2 _enemyLineOfSight;

    [SerializeField]
    protected EnemyBase _enemyData;

    

    [Header("캐릭스펙")]
    protected float _currentMovementSpeed;
    protected float _currentAttackCoolTime;
    protected float _currentMaxHP;
    protected float _currentBodyDamage;
    protected float _currentCriticalChance;
    protected float _dropExpValue;
    protected EnemyType _enemyType;
    protected bool _isBoss;

    protected float _buffMultiplier = 1f;

    //Pattern Spawn 관련
    private bool _isPatternSpawn;


    [Header("무기슬롯관련")]
    [SerializeField]
    protected GameObject _weaponSlot;

    [SerializeField]
    public SpriteRenderer SpriteRenderer;

    protected Animator _enemyAnim; //Not now...

    List<ItemBehaviour> DamageList;


    protected void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
        DamageList = new();
    }

    protected void Start()
    {
        _playerCharacter = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    protected void Update()
    {
        _playerPosition = _playerCharacter.transform.position;
        if(!_isPatternSpawn)
            Movement();
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

        //캐릭터 특정 수치들
        _currentMovementSpeed = EnemyData.SpeedMultiplier * EnemyBase._baseSpeed * _buffMultiplier;
        _currentMaxHP = EnemyData.HP * _buffMultiplier;
        _currentBodyDamage = EnemyData.BodyDamage * _buffMultiplier;
        _currentCriticalChance = EnemyData.CriticalChance;
        _dropExpValue = EnemyData.DropEXP * _buffMultiplier;
        _isBoss = EnemyData.isBossEnemy;

        SpriteRenderer.sprite = EnemyData.EnemySprite;

        transform.localScale = new Vector3(EnemyData.SizeScale, EnemyData.SizeScale, EnemyData.SizeScale);

        if (EnemyData.AnimatorController != null)
            _enemyAnim.runtimeAnimatorController = EnemyData.AnimatorController;

        if (EnemyData.BasicWeaponController != null)
            _weaponSlot = Instantiate(EnemyData.BasicWeaponController, _weaponSlot.transform);
    }

    public void SetForPatternSpawn(SpawnType type)
    {
        _isPatternSpawn = true;

        Vector2 InitialTargetPosition = _playerCharacter.transform.position;

        switch(type)
        {
            case SpawnType.HORDE:
                break;
            case SpawnType.RING:
                break;
        }

    }

    public void HordeMovement(Vector2 targetPosition)
    {

    }


    protected void Movement()
    {
        float step = _currentMovementSpeed * Time.deltaTime;
        transform.position = Vector2.MoveTowards(transform.position, _playerCharacter.transform.position, step);

    }

    protected void UpdateLOS()
    {
        _enemyLineOfSight = (_playerPosition - (Vector2)transform.position).normalized;

        SpriteRenderer.flipX = _enemyLineOfSight.x > 0 ? false : true;
    }

    protected void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameObject player = collision.gameObject;
            PlayerScript script = player.GetComponent<PlayerScript>();
            script.TakeDamage(_currentBodyDamage);
        }
    }

    //protected void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.gameObject.CompareTag("Player"))
    //    {
    //        GameObject player = collision.gameObject;
    //        PlayerScript script = player.GetComponent<PlayerScript>();
    //        script.TakeDamage(_currentBodyDamage);
    //    }
    //}

    public void TakeDamage(float rawDamage, bool isCritical = false, ItemBehaviour damageItem = null, float hitCooldown = 0)
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

            //위에 데미지 뜨는 애니메이션

            GameObject damagePopupPrefab = isCritical ? GameManager.Instance.CriticalDamagePopUpPrefab : GameManager.Instance.DamagePopUpPrefab;

            GameObject DamagePopUp = Instantiate(damagePopupPrefab, transform);
            TextMeshProUGUI tmp = DamagePopUp.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            tmp.text = damage.ToString();

            Rigidbody2D tmpRB = tmp.GetComponent<Rigidbody2D>();

            double jumpRange = (GameManager.Instance.Rand.NextDouble() - 0.5) * 2; 
            //Vector2 direction = new Vector2(UnityEngine.Random.Range(-1, 1), 1);
            Vector2 direction = new Vector2((float)jumpRange, 1);

            float force = isCritical ? 200 : 150;

            tmpRB.AddForce(direction * force);

            Destroy(DamagePopUp, 0.5f);

            CheckDeath();

            if(hitCooldown > 0)
                StartCoroutine(DamageCoroutine(damageItem, hitCooldown));
        }

    }

    private IEnumerator DamageCoroutine(ItemBehaviour damageItem, float hitCooldown)
    {
        DamageList.Add(damageItem);

        yield return new WaitForSeconds(hitCooldown);

        DamageList.Remove(damageItem);
    }

    protected IEnumerator HitAnimation()
    {
        SpriteRenderer.color = Color.red;

        GameObject HitEffect = Instantiate(GameManager.Instance.HitEffectPrefab, transform);

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
        StageManager.Instance.UpdateKill();
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

        GameObject expItem = Instantiate(GameManager.Instance.ExpItemPrefab, RandomNearPosition(), Quaternion.identity);
        ExpItemScript expItemScript = expItem.GetComponent<ExpItemScript>();
        expItemScript.SetExpValue(_dropExpValue);

        //버거소환
        DropBurger();
        //다이아소환
        DropDiamond();

        //Destroy(gameObject);
        StageManager.Instance.EnemyDeathEvent(gameObject, _enemyData);
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
        int randval = GameManager.Instance.Rand.Next(1, 90);

        if (randval <= 1)
        {
            Instantiate(GameManager.Instance.DiamondPrefab, RandomNearPosition(), Quaternion.identity);
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
        _rb.AddForce(LaunchVector, ForceMode2D.Impulse);

        yield return new WaitForSeconds(0.1f);

        _rb.velocity = Vector2.zero;
    }

    public void BuffEnemy()
    {
        _buffMultiplier += 0.2f;
        InitializeWithSO(_enemyData);
    }
}
