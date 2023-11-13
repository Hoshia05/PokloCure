using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public static PlayerScript Instance;


    private PlayerControl _controls; 
    private Rigidbody2D _rb;
    private float _xVelocity;
    private float _yVelocity;
    private bool isInvulnerable = false;

    //private AttackProfile _basicAttack;
    [SerializeField]
    private SpriteRenderer _spriteRenderer;
    [SerializeField]
    private Animator _playerAnim;


    [SerializeField]
    private GameObject _projectilePrefab;

    private float _experience = 0;
    private int _level = 1;

    private float _expRequirement = 79;

    private float _characterCurrentHP;


    [Header("디버깅용")]
    [SerializeField]
    private CharacterBase _characterSO;

    [Header("캐릭스펙")]
    [SerializeField]
    private float _characterMaxHP;
    [SerializeField]
    private float _movementSpeed;
    [SerializeField]
    private float _attackMultiplier;
    [SerializeField]
    private float _criticalChance;


    [Header("숨겨진 수치들")]
    [SerializeField]
    private float _attackCoolTime;
    [SerializeField]
    private float _defensePoints;
    [SerializeField]
    private float _itemEatDistance;


    [SerializeField]
    private GameObject _basicWeaponSlot;
    private WeaponController _basicWeapon;
    [SerializeField]
    private GameObject _obtainedWeaponSlots;
    private int _obtainedWeaponCount = 0;
    [SerializeField]
    private GameObject _obtainedItemSlots;
    private int _obtainedItemCount = 0;



    [Header("소지품")]
    public List<ItemSkillBase> Weapons = new();
    public List<ItemSkillBase> Items = new();

    [Header("기타")]
    [SerializeField]
    private GameObject _cursor;
    [SerializeField]
    private float _cursorDistance;

    private void Awake()
    {
        Instance = this;

        _controls = GetComponent<PlayerControl>();
        _rb = GetComponent<Rigidbody2D>();

        //InitializeWithSO(_characterSO);


    }

    // Start is called before the first frame update
    void Start()
    {
        ExperienceBar.Instance.InitializeEXP(100);
        HPBar.Instance.InitializeHPBar(_characterMaxHP);

        //StartCoroutine(StartAttacking());
    }

    // Update is called once per frame
    void Update()
    {
        EatItemRadius();
        ApplyMovement();
        ApplyFlip();
        CursorUpdate();
    }
    
    public void InitializeFromGM()
    {
        CharacterBase SelectedCharacter = GameManager.Instance.SelectedCharacter;

        InitializeWithSO(SelectedCharacter);
    }

    public void InitializeWithSO(CharacterBase SelectedCharacter)
    {
        //글로벌 특정 수치
        _itemEatDistance = CharacterBase._baseItemEatDistance;

        //캐릭터 특정 수치들
        _movementSpeed = SelectedCharacter.SpeedMultiplier * CharacterBase._baseSpeed;
        _characterMaxHP = SelectedCharacter.Health;
        _characterCurrentHP = _characterMaxHP;
        _attackMultiplier = SelectedCharacter.AttackMultiplier;
        _criticalChance = SelectedCharacter.CriticalChance;

        _defensePoints = SelectedCharacter.DefensePoints;

        _spriteRenderer.sprite = SelectedCharacter.CharacterSprite;
        _playerAnim.runtimeAnimatorController = SelectedCharacter.AnimatorController;

        _basicWeaponSlot = Instantiate(SelectedCharacter.BasicWeaponController, _basicWeaponSlot.transform);
        _basicWeapon = _basicWeaponSlot.GetComponent<WeaponController>();
    }

    void ApplyMovement()
    {
        if (_controls.PlayerMovement != Vector2.zero)
            _playerAnim.SetBool("Move", true);
        else
            _playerAnim.SetBool("Move", false);

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
        while(true)
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

    public void GainEXP(float ExpValue)
    {
        _experience += ExpValue;

        CheckLevelUp();

    }

    private void CheckLevelUp()
    {
        if( _experience >= _expRequirement)
        {
            _level++;
            _experience = _experience - _expRequirement;
            _expRequirement = MathRelated.GetNextExpRequirement(_level);
            ExperienceBar.Instance.LvlUp(_level, _expRequirement);
            StageManager.instance.LevelUpEvent();

        }
        ExperienceBar.Instance.SetEXP(_experience, _level);
    }

    public void ObtainItemSkill(ItemSkillBase item)
    {
        if(item.Type == ItemSkillType.SKILL)
        {
            //TODO:
            //Upgrade Character Skill
        }
        else if(item.Type == ItemSkillType.WEAPON)
        {
            Weapons.Add(item);
        }
        else if(item.Type == ItemSkillType.ITEM)
        {
            Items.Add(item);
        }
    }

    public void TakeDamage(float damage)
    {
        if (!isInvulnerable)
        {
            StartCoroutine(InvulnerabilityCoroutine());

            _characterCurrentHP -= damage;

            HPBar.Instance.UpdateHP(_characterCurrentHP);

            CheckDeath();
        }

    }

    void CheckDeath()
    {
        if(_characterCurrentHP <= 0)
        {
            //Death;
        }
    }

    IEnumerator InvulnerabilityCoroutine()
    {
        isInvulnerable = true;
        _playerAnim.SetTrigger("hurt");

        yield return new WaitForSeconds(0.5f);

        isInvulnerable = false;
        _playerAnim.SetTrigger("hurtEnd");

    }

    public void EatItemRadius()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, _itemEatDistance);

        if(hitColliders.Length > 0 )
        {
            foreach(Collider2D collider in hitColliders)
            {
                if (collider.gameObject.CompareTag("FieldItem"))
                {
                    GameObject Target = collider.gameObject;
                    FieldItemBase fieldItemBase = Target.GetComponent<FieldItemBase>();
                    fieldItemBase.PickedByPlayer(transform);

                }
            }
        }

    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _itemEatDistance);
    }

}
