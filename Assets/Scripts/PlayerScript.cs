﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class PlayerScript : MonoBehaviour
{
    public static PlayerScript Instance;

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
    private HPBar _hpBar;

    private Coroutine _HPBarDisappearCoroutine;

    [SerializeField]
    private GameObject _projectilePrefab;

    //경험치 관련
    private float _currentExperience = 0;
    private int _level = 1;
    private float _expRequirement = 79;


    [Header("디버깅용")]
    [SerializeField]
    private CharacterBase _characterSO;

    [Header("캐릭스펙")]
    private float _currentCharacterMaxHP;
    private float _characterCurrentHP;
    private float _currentSpeedMultiplier = 0;
    private float _currentMovementSpeed => _currentSpeedMultiplier * CharacterBase._baseSpeed;
    private float _currentAttackMultiplier;
    public float CurrentAttackMultiplier
    {
        get
        {
            return _currentAttackMultiplier;
        }
    }
    private float _currentCriticalMultiplier;
    private float _currentCriticalChance => _currentCriticalMultiplier + CharacterBase._baseCriticalChance;
    private float _currentCriticalDamage;

    private CharacterBase _selectedCharacter;

    [Header("숨겨진 수치들")]
    [SerializeField]
    private float _attackCoolTime;
    [SerializeField]
    private float _defensePoints;
    [SerializeField]
    private float _eatDistanceMultiplier = 1f;
    private float _itemEatDistance => CharacterBase._baseItemEatDistance * _eatDistanceMultiplier;

    [Header("무기슬롯관련")]
    [SerializeField]
    private GameObject _basicWeaponSlot;
    private ItemController _basicWeapon;
    [SerializeField]
    private GameObject _obtainedWeaponSlots;

    private Dictionary<ItemSO, ItemController> _weapons = new();
    public Dictionary<ItemSO, ItemController> Weapons
    {
        get { return _weapons; }
    }
    private int _obtainedWeaponCount = 0;

    [SerializeField]
    private GameObject _obtainedItemSlots;
    private Dictionary<ItemSO, ItemController> _items = new();
    public Dictionary<ItemSO, ItemController> Items
    {
        get { return _items; }
    }
    private int _obtainedItemCount = 0;

    [SerializeField]
    private GameObject _skillSlots;
    private Dictionary<ItemSO, ItemController> _skills = new();
    public Dictionary<ItemSO, ItemController> Skills
    {
        get { return _skills; }
    }

    private const int MAXSLOT = 7;

    [Header("이벤트")]
    [HideInInspector]
    public UnityEvent onEatBurger;
    public UnityEvent onStatChange;

    [Header("기타")]
    [SerializeField]
    private GameObject _cursor;
    [SerializeField]
    private float _cursorDistance;

    private void Awake()
    {
        Instance = this;

        _rb = GetComponent<Rigidbody2D>();

        InitializeFromGM();

    }

    // Start is called before the first frame update
    void Start()
    {
        ExperienceBar.Instance.InitializeEXP(100);
        HPBar.Instance.InitializeHPBar(_currentCharacterMaxHP);

        _hpBar.gameObject.SetActive(false);

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
        _selectedCharacter = GameManager.Instance.SelectedCharacter;

        InitializeWithSO(_selectedCharacter);

    }

    public void InitializeWithSO(CharacterBase SelectedCharacter)
    {
        //글로벌 특정 수치
        //_itemEatDistance = CharacterBase._baseItemEatDistance;

        //캐릭터 특정 수치들
        _currentSpeedMultiplier = SelectedCharacter.SpeedMultiplier;
        //_currentMovementSpeed = _currentSpeedMultiplier * CharacterBase._baseSpeed;
        _currentCharacterMaxHP = SelectedCharacter.Health;
        _characterCurrentHP = _currentCharacterMaxHP;
        _currentAttackMultiplier = SelectedCharacter.AttackMultiplier;
        _currentCriticalMultiplier = SelectedCharacter.CriticalMultiplier;
        _currentCriticalDamage = CharacterBase._baseCritDamage;

        //_defensePoints = SelectedCharacter.DefensePoints;

        _spriteRenderer.sprite = SelectedCharacter.CharacterSprite;
        _playerAnim.runtimeAnimatorController = SelectedCharacter.AnimatorController;

        _basicWeaponSlot = Instantiate(SelectedCharacter.BasicWeaponController, _basicWeaponSlot.transform);
        _basicWeapon = _basicWeaponSlot.GetComponent<ItemController>();

        UpdateInfoUI();
    }

    public void UpdateInfoUI()
    {
        CharacterInfoUIScript.Instance.SetCharacterPortrait(_selectedCharacter.CharacterPortrait);
        CharacterInfoUIScript.Instance.SetCharacterName(_selectedCharacter.CharacterName);
        CharacterInfoUIScript.Instance.SetHP(_characterCurrentHP, _currentCharacterMaxHP);
        CharacterInfoUIScript.Instance.SetAtk(_currentAttackMultiplier);
        CharacterInfoUIScript.Instance.SetSpd(_currentSpeedMultiplier);
        CharacterInfoUIScript.Instance.SetCrt(_currentCriticalMultiplier);
        CharacterInfoUIScript.Instance.SetPickup(_eatDistanceMultiplier);
        CharacterInfoUIScript.Instance.SetHaste(_eatDistanceMultiplier);
    }

    void ApplyMovement()
    {
        if (PlayerControl.Instance.PlayerMovement != Vector2.zero)
            _playerAnim.SetBool("Move", true);
        else
            _playerAnim.SetBool("Move", false);

        _xVelocity = PlayerControl.Instance.PlayerMovement.x * _currentMovementSpeed;
        _yVelocity = PlayerControl.Instance.PlayerMovement.y * _currentMovementSpeed;

        _rb.velocity = new Vector2(_xVelocity, _yVelocity);
    }

    void CursorUpdate()
    {
        Vector2 cursorVector = PlayerControl.Instance.PlayerLineOfSight * _cursorDistance;

        _cursor.transform.position = (Vector2)transform.position + cursorVector;
    }

    void ApplyFlip()
    {
        if(PlayerControl.Instance.PlayerLineOfSight.x >= 0)
        {
            _spriteRenderer.flipX = true;
        }
        else
        {
            _spriteRenderer.flipX = false; ;
        }
    }

    public void GainEXP(float ExpValue)
    {
        _currentExperience += ExpValue;

        CheckLevelUp();

    }

    private void CheckFull()
    {
        if(_currentCharacterMaxHP >= _characterCurrentHP)
        {
            _HPBarDisappearCoroutine = StartCoroutine(DisappearHealthBar());
        }
    }

    private IEnumerator DisappearHealthBar()
    {
        yield return new WaitForSeconds(2f);

        _hpBar.gameObject.SetActive(false);
    }

    public void HealHP(float HealValue, bool IsBasicBurger)
    {
        if (IsBasicBurger)
        {
            _characterCurrentHP += _currentCharacterMaxHP * 0.2f;
            onEatBurger.Invoke();
        }
        else
        {
            _characterCurrentHP += HealValue;
        }


        if (_characterCurrentHP > _currentCharacterMaxHP)
            _characterCurrentHP = _currentCharacterMaxHP;

        CheckFull();

        HPBar.Instance.UpdateHP(_characterCurrentHP);
    }

    public void AttackStatChange(float buffValue)
    {
        _currentAttackMultiplier = _currentAttackMultiplier + buffValue;
        onStatChange.Invoke();
    }

    private void CheckLevelUp()
    {
        if( _currentExperience >= _expRequirement)
        {
            _level++;
            _currentExperience = _currentExperience - _expRequirement;
            _expRequirement = MathRelated.GetNextExpRequirement(_level);
            ExperienceBar.Instance.LvlUp(_level, _expRequirement);
            StageManager.instance.LevelUpEvent();

        }
        ExperienceBar.Instance.SetEXP(_currentExperience, _level);
    }

    public ItemController CheckItemPossession(ItemSO item)
    {
        if (_basicWeapon.ItemData == item)
        {
            return _basicWeapon;
        }
        else if (_weapons.ContainsKey(item))
        {
            return _weapons[item];
        }
        else if (_items.ContainsKey(item))
        {
            return _items[item];
        }
        else if (_skills.ContainsKey(item))
        {
            return _skills[item];
        }
        else
        {
            return null;
        }
    }

    public int CheckItemPossessionLevel(ItemSO item)
    {
        ItemController itemController = CheckItemPossession(item);

        if(itemController != null)
        {
            return itemController.CurrentWeaponLevel + 1;
        }
        else
        {
            return 1;
        }

    }

    public void ObtainItemSkill(ItemSO item)
    {
        ItemController itemController = CheckItemPossession(item);
        if (itemController != null)
        {
            itemController.LevelUp();
        }
        else
        {
            if (item.ItemType == ItemType.SKILL)
            {
                //TODO:
                //Upgrade Character Skill
                GameObject NewWeapon = Instantiate(item.ControllerPrefab, _obtainedWeaponSlots.transform);
                ItemController newSkillController = NewWeapon.GetComponent<ItemController>();
                _skills.Add(item, newSkillController);
            }
            else if (item.ItemType == ItemType.WEAPON)
            {
                GameObject NewWeapon = Instantiate(item.ControllerPrefab, _obtainedWeaponSlots.transform);
                ItemController newWeaponController = NewWeapon.GetComponent<ItemController>();

                //GameObject NewWeapon = new GameObject(item.ItemName);
                //NewWeapon.transform.SetParent(_obtainedWeaponSlots.transform);

                //var type = item.ControllerScript.GetType();

                //NewWeapon.AddComponent(typeof(item.ControllerScript));
                //ItemController newWeaponController = NewWeapon.GetComponent<ItemController>();
                //newWeaponController.SetWithSO(item);

                _weapons.Add(item, newWeaponController);
                _obtainedWeaponCount++;
            }
            else if (item.ItemType == ItemType.ITEM)
            {
                GameObject NewItem = Instantiate(item.ControllerPrefab, _obtainedItemSlots.transform);
                ItemController newItemController = NewItem.GetComponent<ItemController>();
                _items.Add(item, newItemController);
                _obtainedItemCount++;
            }
        }
    }

    public bool CriticalCheck()
    {
        int checkNum = (int)(_currentCriticalChance * 100);

        if(GameManager.Instance.Rand.Next(0, 100) < checkNum)
            return true;
        else
            return false;
    }

    public float GetCritDamage(float baseDamage)
    {
        return ItemController.RoundValue(baseDamage * _currentCriticalDamage);
    }

    public void TakeDamage(float damage)
    {
        if (!isInvulnerable)
        {
            StartCoroutine(InvulnerabilityCoroutine());

            _characterCurrentHP -= damage;

            HPBar.Instance.UpdateHP(_characterCurrentHP);

            if (_HPBarDisappearCoroutine != null)
                StopCoroutine(_HPBarDisappearCoroutine);

            _hpBar.gameObject.SetActive(true);

            CheckDeath();
        }

    }

    void CheckDeath()
    {
        if(_characterCurrentHP <= 0)
        {
            Destroy(gameObject);
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
