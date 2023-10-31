using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private PlayerControl _controls; 
    private Rigidbody2D _rb;
    private float _xVelocity;
    private float _yVelocity;

    [SerializeField]
    private SpriteRenderer _spriteRenderer;
    [SerializeField]
    private GameObject _cursor;


    [SerializeField]
    private GameObject _projectilePrefab;

    private bool isInvulnerable = false;
    private float _experience = 0;
    private int _level = 1;

    [Header("캐릭스펙")]
    [SerializeField]
    private float _movementSpeed;
    [SerializeField]
    private float _attackCoolTime;
    [SerializeField]
    private float _characterHP;
    [SerializeField]
    private float _defensePoints;
    [SerializeField]
    private float _attackMultiplier;

    [Header("세팅")]
    [SerializeField]
    private float _cursorDistance;


    // Start is called before the first frame update
    void Start()
    {
        _controls = GetComponent<PlayerControl>();
        _rb = GetComponent<Rigidbody2D>();

        StatInitialize();

        StartCoroutine(StartAttacking());
    }

    // Update is called once per frame
    void Update()
    {
        ApplyMovement();
        ApplyFlip();
        CursorUpdate();
    }   


    void ApplyMovement()
    {
        _xVelocity = _controls.PlayerMovement.x * _movementSpeed;
        _yVelocity = _controls.PlayerMovement.y * _movementSpeed;

        _rb.velocity = new Vector2(_xVelocity, _yVelocity);
    }

    void CursorUpdate()
    {
        Vector2 cursorVector = _controls.PlayerLineOfSight * _cursorDistance;

        _cursor.transform.position = (Vector2)transform.position + cursorVector;
    }

    void ApplyFlip()
    {
        if(_controls.PlayerLineOfSight.x >= 0)
        {
            _spriteRenderer.flipX = true;
        }
        else
        {
            _spriteRenderer.flipX = false; ;
        }
    }

    IEnumerator StartAttacking()
    {
        for(; ; )
        {
            yield return new WaitForSeconds(_attackCoolTime);
            AttackAction();

        }
    }

    void AttackAction()
    {
        GameObject projectile = Instantiate(_projectilePrefab, transform);
        Rigidbody2D projectileRB = projectile.GetComponent<Rigidbody2D>();
        projectileRB.AddForce(_controls.PlayerLineOfSight * 1000);

    }

    void StatInitialize()
    {

    }

    public void GainEXP(float ExpValue)
    {
        _experience += ExpValue;

        CheckLevelUp();


    }

    private void CheckLevelUp()
    {
        //todo
    }

    public void TakeDamage(float damage)
    {
        if (!isInvulnerable)
        {
            StartCoroutine(InvulnerabilityCoroutine());

            _characterHP -= damage;

            CheckDeath();
        }

    }

    void CheckDeath()
    {
        if(_characterHP <= 0)
        {
            //Death;
        }
    }

    IEnumerator InvulnerabilityCoroutine()
    {
        isInvulnerable = true;

        yield return new WaitForSeconds(0.5f);

        isInvulnerable = false;

    }
}
