﻿using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    // Start is called before the first frame update

    protected GameObject _playerCharacter;
    protected Vector2 _playerPosition;
    protected Rigidbody2D _rb;

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
    //private int _enemyLevel = 1;


    [Header("무기슬롯관련")]
    [SerializeField]
    protected GameObject _weaponSlot;

    [SerializeField]
    protected SpriteRenderer _spriteRenderer;


    protected Animator _enemyAnim; //Not now...

    protected void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    protected void Start()
    {
        _playerCharacter = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    protected void Update()
    {
        _playerPosition = _playerCharacter.transform.position;
        Movement();
        UpdateLOS();
    }

    public void InitializeWithSO(EnemyBase EnemyData)
    {
        //캐릭터 특정 수치들
        _currentMovementSpeed = EnemyData.SpeedMultiplier * EnemyBase._baseSpeed;
        _currentMaxHP = EnemyData.HP;
        _currentBodyDamage = EnemyData.BodyDamage;
        _currentCriticalChance = EnemyData.CriticalChance;
        _dropExpValue = EnemyData.DropEXP;
        _isBoss = EnemyData.isBossEnemy;

        _spriteRenderer.sprite = EnemyData.EnemySprite;


        if (EnemyData.AnimatorController != null)
            _enemyAnim.runtimeAnimatorController = EnemyData.AnimatorController;

        if (EnemyData.BasicWeaponController != null)
            _weaponSlot = Instantiate(EnemyData.BasicWeaponController, _weaponSlot.transform);
    }


    protected void Movement()
    {
        float step = _currentMovementSpeed * Time.deltaTime;
        transform.position = Vector2.MoveTowards(transform.position, _playerCharacter.transform.position, step);

    }

    protected void UpdateLOS()
    {
        _enemyLineOfSight = (_playerPosition - (Vector2)transform.position).normalized;

        _spriteRenderer.flipX = _enemyLineOfSight.x > 0 ? true : false;
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameObject player = collision.gameObject;
            PlayerScript script = player.GetComponent<PlayerScript>();
            script.TakeDamage(_currentBodyDamage);
        }
    }

    public void TakeDamage(float rawDamage)
    {
        float damage = DamageVariance(rawDamage);

        _currentMaxHP -= damage;

        //적 피격 모션 : 간단하게 한 0.3초 동안 빨갛게 되기
        StartCoroutine(HitAnimation());

        //위에 데미지 뜨는 애니메이션
        GameObject DamagePopUp = Instantiate(GameManager.Instance.DamagePopUpPrefab, transform);
        TextMeshProUGUI tmp = DamagePopUp.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        tmp.text = damage.ToString();

        Rigidbody2D tmpRB = tmp.GetComponent<Rigidbody2D>();

        Vector2 direction = new Vector2(UnityEngine.Random.Range(-1,1), 1);
        tmpRB.AddForce(direction * 150);

        Destroy(DamagePopUp, 0.5f);
        
        CheckDeath();

    }

    public void TakeCriticalDamage(float rawDamage)
    {

        float damage = DamageVariance(rawDamage);

        _currentMaxHP -= damage;

        //적 피격 모션 : 간단하게 한 0.3초 동안 빨갛게 되기
        StartCoroutine(HitAnimation());

        //위에 데미지 뜨는 애니메이션
        GameObject DamagePopUp = Instantiate(GameManager.Instance.CriticalDamagePopUpPrefab, transform);
        TextMeshProUGUI tmp = DamagePopUp.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        tmp.text = damage.ToString();

        Rigidbody2D tmpRB = tmp.GetComponent<Rigidbody2D>();

        Vector2 direction = new Vector2(UnityEngine.Random.Range(-1, 1), 1);
        tmpRB.AddForce(direction * 200);

        Destroy(DamagePopUp, 0.5f);

        CheckDeath();
    }

    protected IEnumerator HitAnimation()
    {
        _spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.4f);
        _spriteRenderer.color = Color.white;
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
        StageManager.instance.UpdateKill();
        Collider2D coll = GetComponent<Collider2D>();
        coll.enabled = false;

        yield return new WaitForSeconds(0.1f);

        _currentMovementSpeed = 0;
        Color originalColor = _spriteRenderer.color;

        float elapsedTime = 0f;

        while (elapsedTime < 0.5f)
        {
            float alpha = Mathf.Lerp(originalColor.a, 0f, elapsedTime / 0.5f);

            _spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        _spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);

        GameObject expItem = Instantiate(GameManager.Instance.ExpItemPrefab, RandomNearPosition(), Quaternion.identity);
        ExpItemScript expItemScript = expItem.GetComponent<ExpItemScript>();
        expItemScript.SetExpValue(_dropExpValue);

        //버거소환
        DropBurger();
        //다이아소환
        DropDiamond();

        Destroy(gameObject);
        StageManager.instance.CurrentEnemyCount--;

    }

    protected Vector2 RandomNearPosition()
    {
        return new Vector2(transform.position.x + GameManager.Instance.Rand.Next(1, 10) * 0.1f, transform.position.y + GameManager.Instance.Rand.Next(1, 10) * 0.1f);
    }

    protected void DropBurger()
    {
        int randval = GameManager.Instance.Rand.Next(1, 100);

        if(randval <= 5) 
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

}