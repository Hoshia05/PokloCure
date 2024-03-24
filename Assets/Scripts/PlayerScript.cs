using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using static UnityEditor.Progress;

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
    public float CurrentCharacterMaxHP
    {
        get => _currentCharacterMaxHP; 
    }
    private float _characterCurrentHP;

    private float _currentMaxStamina;
    private float _currentStamina;

    private float _currentSpeedMultiplier = 0;
    private float _currentMovementSpeed => _currentSpeedMultiplier * CharacterBase._baseSpeed;
    private float _currentAttackMultiplier;
    public float CurrentAttackMultiplier
    {
        get => _currentAttackMultiplier;
    }
    private float _currentCriticalMultiplier;
    private float _currentCriticalChance => _currentCriticalMultiplier + CharacterBase._baseCriticalChance;
    private float _currentCriticalDamage;
    private CharacterDistinct _characterLabel;
    public CharacterDistinct CharacterLabel
    {
        get => _characterLabel; 
    }
    private float _currentHasteMultiplier;
    public float CurrentHasteMultiplier
    {
        get => _currentHasteMultiplier;
    }
    private float _currentDamageReducePercentage;
    private float _currentDamageReduceValue;

    private float _currentAttackSizeBuff;
    public float CurrentAttackSizeBuff
    {
        get => _currentAttackSizeBuff;
    }

    private float _currentKnockbackBuff;
    public float CurrentKnockbackBuff
    {
        get => _currentKnockbackBuff;
    }

    private float _eatDistanceMultiplier;

    public float BurgerDropChanceMultiplier;

    private int _rangedProjectileBuff;
    public int RangedProjectileBuff { get => _rangedProjectileBuff; }

    private float _meleeCooldownBuff;
    public float MeleeCooldownBuff { get => _meleeCooldownBuff; }

    private float _rangedCooldownBuff;
    public float RangedCooldownBuff { get => _rangedCooldownBuff; }

    //실드
    private float _shield;

    //스테미나
    private float _staminaReplenishRate;
    private float _staminaDepleteRate;
    private float _staminaRedlineValue;

    //버프 관리용 Dictionary
    public Dictionary<ItemController, BuffObject> BuffDictionary = new();

    private CharacterBase _selectedCharacter;

    [Header("숨겨진 수치들")]
    [SerializeField]
    private float _attackCoolTime;
    [SerializeField]
    private float _defensePoints;
    private float _itemEatDistance => CharacterBase._baseItemEatDistance * _eatDistanceMultiplier;

    private bool _redlineCharging;

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
    public int ObtainedWeaponCount { get { return _obtainedWeaponCount; } }

    [SerializeField]
    private int _weaponSlotCount = 3;
    public int WeaponSlotCount { get { return _weaponSlotCount; } }

    [SerializeField]
    private GameObject _obtainedItemSlots;
    private Dictionary<ItemSO, ItemController> _items = new();
    public Dictionary<ItemSO, ItemController> Items
    {
        get { return _items; }
    }
    private int _obtainedItemCount = 0;
    public int ObtainedItemCount { get { return _obtainedItemCount; } }

    [SerializeField]
    private int _itemSlotCount = 3;
    public int ItemSlotCount { get { return _itemSlotCount; } }

    private const int MAXSLOTCOUNT = 7;


    [SerializeField]
    private GameObject _upgradeSlots;

    [SerializeField]
    private GameObject _skillSlots;
    private Dictionary<ItemSO, ItemController> _skills = new();
    public Dictionary<ItemSO, ItemController> Skills
    {
        get { return _skills; }
    }


    [Header("이벤트")]
    [HideInInspector]
    public UnityEvent onEatBurger;
    public UnityEvent onStatChange;
    public UnityEvent<float> onMaxHPChange;

    public UnityEvent onShieldDamage;
    public delegate T DamageTakenDelegate<T>();
    public event DamageTakenDelegate<bool> OnDamageTakenBool;
    public UnityEvent onTakeDamage;

    [Header("기타")]
    [SerializeField]
    private GameObject _cursor;
    [SerializeField]
    private float _cursorDistance;

    public Coroutine DamageCoroutine;

    public bool IsPaused;
    public bool IsDead;


    private void Awake()
    {
        Instance = this;

        _rb = GetComponent<Rigidbody2D>();

        InitializeFromGM();

        _shield = 0;

    }

    // Start is called before the first frame update
    void Start()
    {
        ExperienceBar.Instance.InitializeEXP(100);
        HPBar.Instance.InitializeHPBar(_currentCharacterMaxHP);
        UIStaminaBar.Instance.InitializeStaminaBar(_currentMaxStamina);

        _hpBar.gameObject.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        if (!IsPaused && !IsDead)
        {
            EatItemRadius();
            ApplyMovement();
            ApplyFlip();
            CursorUpdate();
            StaminaReplenish();
        }
    }
    
    public void InitializeFromGM()
    {
        _selectedCharacter = GameManager.Instance.SelectedCharacter;

        _characterSO = _selectedCharacter;

        InitializeWithSO(_selectedCharacter);

    }

    public void InitializeWithSO(CharacterBase SelectedCharacter)
    {
        //글로벌 특정 수치
        //_itemEatDistance = CharacterBase._baseItemEatDistance;

        //캐릭터 특정 수치들
        _currentSpeedMultiplier = SelectedCharacter.SpeedMultiplier;
        _currentCharacterMaxHP = _characterCurrentHP = SelectedCharacter.Health;
        _currentMaxStamina = _currentStamina = SelectedCharacter.Stamina;
        _currentAttackMultiplier = SelectedCharacter.AttackMultiplier;
        _currentCriticalMultiplier = SelectedCharacter.CriticalMultiplier;
        _currentCriticalDamage = CharacterBase._baseCritDamage;
        _currentHasteMultiplier = CharacterBase._baseHaste;
        _currentDamageReducePercentage = 1f;
        _currentDamageReduceValue = 0f;

        _currentAttackSizeBuff = 1f;
        _currentKnockbackBuff = 1f;
        _eatDistanceMultiplier = 1f;

        _meleeCooldownBuff = 1f;
        _rangedCooldownBuff = 1f;

        BurgerDropChanceMultiplier = 0f;

        _characterLabel = SelectedCharacter.characterLabel;

        _spriteRenderer.sprite = SelectedCharacter.CharacterSprite;
        _playerAnim.runtimeAnimatorController = SelectedCharacter.AnimatorController;


        _staminaReplenishRate = SelectedCharacter.StaminaReplenishRate;
        _staminaDepleteRate = SelectedCharacter.StaminaDepleteRate;
        _staminaRedlineValue = SelectedCharacter.StaminaRedlineValue;

        GameObject NewWeapon = new GameObject(SelectedCharacter.BaseWeapon.ItemName);
        NewWeapon.transform.parent = _basicWeaponSlot.transform;
        NewWeapon.transform.localPosition = Vector3.zero;

        System.Type scriptType = (SelectedCharacter.BaseWeapon.ControllerScript as MonoScript).GetClass();
        _basicWeapon = NewWeapon.AddComponent(scriptType) as ItemController;
        _basicWeapon.SetWithSO(SelectedCharacter.BaseWeapon);

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

    void StaminaReplenish()
    {
        if(!PlayerControl.Instance.IsDashing && _currentStamina < _currentMaxStamina && !_redlineCharging)
        {
            _currentStamina += Time.deltaTime * _staminaReplenishRate;
            UIStaminaBar.Instance.UpdateStamina(_currentStamina);
        }
        else if(_currentStamina <= 0)
        {
            StartCoroutine(StaminaReplenishCoroutine());
        }
    }

    IEnumerator StaminaReplenishCoroutine()
    {
        _redlineCharging = true;
        _currentStamina = 0;
        UIStaminaBar.Instance.StartRedline();

        yield return new WaitForSeconds(1.5f);

        UIStaminaBar.Instance.EndRedline();
        _redlineCharging = false;
        _currentStamina = _staminaRedlineValue;
    }

    void ApplyMovement()
    {
        if (PlayerControl.Instance.GetPlayerMovement() != Vector2.zero)
            _playerAnim.SetBool("Move", true);
        else
            _playerAnim.SetBool("Move", false);

        float currentMovement;

        if (PlayerControl.Instance.IsDashing && _currentStamina > 0)
        {
            currentMovement = _currentMovementSpeed * _selectedCharacter.DashMultiplier;
            _currentStamina -= Time.deltaTime * _staminaDepleteRate;
            UIStaminaBar.Instance.UpdateStamina(_currentStamina);
        }
        else
        {
            currentMovement = _currentMovementSpeed;
        }

        _xVelocity = PlayerControl.Instance.GetPlayerMovement().x * currentMovement;
        _yVelocity = PlayerControl.Instance.GetPlayerMovement().y * currentMovement;

        _rb.velocity = new Vector2(_xVelocity, _yVelocity);
    }

    void CursorUpdate()
    {
        Vector2 cursorVector = PlayerControl.Instance.PlayerLineOfSight * _cursorDistance;

        _cursor.transform.position = (Vector2)transform.position + cursorVector;


        Vector2 AttackDirection = PlayerControl.Instance.PlayerLineOfSight;

        float angle = Mathf.Atan2(AttackDirection.y, AttackDirection.x) * Mathf.Rad2Deg;
        _cursor.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    void ApplyFlip()
    {
        if(PlayerControl.Instance.PlayerLineOfSight.x >= 0)
        {
            _spriteRenderer.flipX = false;
        }
        else
        {
            _spriteRenderer.flipX = true; ;
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

    public void HealStamina(float HealValue)
    {
        _currentStamina += HealValue;
        UIStaminaBar.Instance.UpdateStamina(_currentStamina);
    }

    public void UpdateBuffDictionary(ItemController itemController, BuffObject buffInfo)
    {
        BuffObject buffResult;

        bool contains = BuffDictionary.TryGetValue(itemController, out buffResult);
        
        if (contains)
        {
            BuffDictionary[itemController] = buffInfo;
        }
        else
        {
            BuffDictionary.Add(itemController, buffInfo);
        }

        ApplyBuffs();
        UpdateInfoUI();
    }

    private void ApplyBuffs()
    {
        _currentAttackMultiplier = _selectedCharacter.AttackMultiplier;
        _currentCharacterMaxHP = _selectedCharacter.Health;
        _currentSpeedMultiplier = _selectedCharacter.SpeedMultiplier;
        _currentCriticalMultiplier = _selectedCharacter.CriticalMultiplier;
        _currentCriticalDamage = CharacterBase._baseCritDamage;
        _currentHasteMultiplier = CharacterBase._baseHaste;
        _currentDamageReducePercentage = 1f;
        _currentDamageReduceValue = 0f;

        _currentAttackSizeBuff = 1f;
        _currentKnockbackBuff = 1f;
        _eatDistanceMultiplier = 1f;

        BurgerDropChanceMultiplier = 0f;

        _rangedProjectileBuff = 0;
        _rangedCooldownBuff = 1f;
        _meleeCooldownBuff = 1f;


        foreach (BuffObject buff in BuffDictionary.Values)
        {
            _currentCharacterMaxHP += buff.HPBuff;
            _currentAttackMultiplier += buff.AttackMultiplierBuff;
            _currentSpeedMultiplier += buff.SpeedMultiplierBuff;
            _currentCriticalMultiplier += buff.CritMultiplierBuff;
            _currentCriticalDamage += buff.CritDamageBuff;
            _currentHasteMultiplier -= buff.HasteMultiplierBuff;
            _currentDamageReducePercentage -= buff.DamageReductionPercentage;
            _currentDamageReduceValue += buff.DamageReductionValue;

            _currentAttackSizeBuff += buff.AttackSizeBuff;
            _currentKnockbackBuff += buff.KnockBackBuff;
            _eatDistanceMultiplier += buff.EatDistanceMultiplier;

            BurgerDropChanceMultiplier += buff.BurgerDropChance;

            _rangedProjectileBuff += buff.RangedProjectileBuff;
            _rangedCooldownBuff -= buff.RangedCooldownBuff;
            _meleeCooldownBuff -= buff.MeleeCooldownBuff;
        }


        onMaxHPChange.Invoke(CurrentCharacterMaxHP);
        onStatChange.Invoke();
    }

    public void AttackStatChange(float buffValue, float PreviousIncrement = 0)
    {
        _currentAttackMultiplier = _currentAttackMultiplier - PreviousIncrement;
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
            StageManager.Instance.LevelUpEvent();

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
            return itemController.CurrentLevel + 1;
        }
        else
        {
            return 1;
        }

    }

    public bool IncreaseWeaponSlot()
    {
        if(WeaponSlotCount == MAXSLOTCOUNT)
        {
            return false;
        }
        else
        {
            _weaponSlotCount++;
            ItemSlotScript.Instance.SlotUpdate(this);
            return true;
        }
    }

    public bool IncreaseItemSlot()
    {
        if (ItemSlotCount == MAXSLOTCOUNT)
        {
            return false;
        }
        else
        {
            _itemSlotCount++;
            ItemSlotScript.Instance.SlotUpdate(this);
            return true;
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
                GameObject NewSkill = new GameObject(item.ItemName);
                NewSkill.transform.parent = _skillSlots.transform;
                NewSkill.transform.localPosition = Vector3.zero;

                System.Type scriptType = (item.ControllerScript as MonoScript).GetClass();
                ItemController controllerScript = NewSkill.AddComponent(scriptType) as ItemController;
                controllerScript.SetWithSO(item);
                _skills.Add(item, controllerScript);
            }
            else if (item.ItemType == ItemType.WEAPON)
            {
                GameObject NewWeapon = new GameObject(item.ItemName);
                NewWeapon.transform.parent = _obtainedWeaponSlots.transform;
                NewWeapon.transform.localPosition = Vector3.zero;

                System.Type scriptType = (item.ControllerScript as MonoScript).GetClass();
                ItemController controllerScript = NewWeapon.AddComponent(scriptType) as ItemController;
                controllerScript.SetWithSO(item);
                _weapons.Add(item, controllerScript);
                _obtainedWeaponCount++;
            }
            else if (item.ItemType == ItemType.ITEM)
            {
                GameObject NewItem = new GameObject(item.ItemName);
                NewItem.transform.parent = _obtainedItemSlots.transform;
                NewItem.transform.localPosition = Vector3.zero;

                System.Type scriptType = (item.ControllerScript as MonoScript).GetClass();
                ItemController controllerScript = NewItem.AddComponent(scriptType) as ItemController;
                controllerScript.SetWithSO(item);
                _items.Add(item, controllerScript);
                _obtainedItemCount++;
            }
            else if (item.ItemType == ItemType.UPGRADE)
            {
                GameObject NewUpgrade = new GameObject(item.ItemName);
                NewUpgrade.transform.parent = _upgradeSlots.transform;
                NewUpgrade.transform.localPosition = Vector3.zero;

                System.Type scriptType = (item.ControllerScript as MonoScript).GetClass();
                ItemController controllerScript = NewUpgrade.AddComponent(scriptType) as ItemController;
                controllerScript.SetWithSO(item);
                _skills.Add(item, controllerScript);
            }
        }
    }

    

    public void SetShield(float shieldValue)
    {
        _shield = shieldValue;
        UIShieldBar.Instance.NewShieldValue(_shield);
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
        //데미지 받는 단계
        // 무적상태 확인 -> 템중에서 피격 무효화 확인 -> 쉴드 데미지 -> HP 데미지 -> 데미지 받음 Invoke

        if (isInvulnerable)
            return;

        if (ResolveDamageNegation(OnDamageTakenBool))
            return;

        float ActualDamage = damage * _currentDamageReducePercentage - _currentDamageReduceValue;

        if(_shield > 0)
        {
            //실드 먼저 데미지
            onShieldDamage.Invoke();
            float leftoverDamage = ActualDamage - _shield;

            if (leftoverDamage > 0)
            {
                _shield = 0;

                UIShieldBar.Instance.UpdateShield(_shield);

                onTakeDamage.Invoke();
                DamageCoroutine = StartCoroutine(TakeDamageCoroutine(leftoverDamage));
            }
            else
            {
                _shield -= ActualDamage;
                UIShieldBar.Instance.UpdateShield(_shield);
            }

        }
        else
        {
            onTakeDamage.Invoke();
            DamageCoroutine = StartCoroutine(TakeDamageCoroutine(ActualDamage));
        }


    }

    private bool ResolveDamageNegation(DamageTakenDelegate<bool> OnDamageTakenBool)
    {
        if (OnDamageTakenBool != null)
        {
            foreach (DamageTakenDelegate<bool> handler in OnDamageTakenBool.GetInvocationList())
            {
                bool feedback = handler.Invoke();

                if (feedback)
                    return true;
            }
        }
        return false;
    }


    IEnumerator TakeDamageCoroutine(float damage)
    {
        isInvulnerable = true;
        _playerAnim.SetTrigger("hurt");
        _spriteRenderer.color = Color.red;

        _characterCurrentHP -= damage;

        HPBar.Instance.UpdateHP(_characterCurrentHP);

        if (_HPBarDisappearCoroutine != null)
            StopCoroutine(_HPBarDisappearCoroutine);

        _hpBar.gameObject.SetActive(true);

        CheckDeath();

        yield return new WaitForSeconds(0.5f);

        _spriteRenderer.color = Color.white;
        isInvulnerable = false;
        _playerAnim.SetTrigger("hurtEnd");

    }

    void CheckDeath()
    {
        if (_characterCurrentHP <= 0)
        {
            IsDead = true;
            StageManager.Instance.GameOverEvent();
            Destroy(gameObject);
        }
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
