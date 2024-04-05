using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour, IItemController
{
    public ItemSO ItemData;

    protected float _currentDamage;
    protected float _currentSpeed;
    protected float _currentCooldownDuration;
    protected int _currentPierce;
    protected float _currentDeathtime;

    protected float _currentCooldown;
    protected float _currentSizeScale = 1;

    protected float _currentKnockbackValue = 1;

    protected float _localDamageBuff = 0f;
    protected float _localSpeedBuff = 0f;
    protected float _localSizeBuff = 0f;
    protected float _localCooldownBuff = 1f;
    protected float _localDeathTimebuff = 0;

    protected int _projectileNum = 1;
    protected int _additionalProjectiles = 0;

    protected int _buffStack = 0;
    protected int _maxStack = 0;

    protected int _additionalPierce = 0;
    protected float _additionalKnockbackValue = 0;

    protected float _stunTime = 0f;

    public List<ItemBehaviour> CurrentProjectiles = new();

    protected delegate void LevelUPEffects();
    protected List<LevelUPEffects> _levelUPEffectsList;

    protected BuffObject _buff = new();

    protected bool _hasLaunchedSinceReset;

    protected bool _cooldownWaitUntilProjectileDeath;
    protected bool _weaponDestroyedAfterPierce;

    protected int _currentLevel = 1;
    public int CurrentLevel
    {
        get { return _currentLevel; }
        set { _currentLevel = value; }
    }

    // Start is called before the first frame update
    protected virtual void Awake()
    {
        if(ItemData != null)
        {
            SetWithSO(ItemData);
        }

        PlayerScript.Instance.onStatChange.AddListener(ApplyStats);
    }

    public void SetWithSO(ItemSO itemData)
    {
        ItemData = itemData;

        ApplyStats();

        _currentCooldown = 0.1f;

        _levelUPEffectsList = new List<LevelUPEffects>();
        _levelUPEffectsList.Add(Level1Effect);
        _levelUPEffectsList.Add(Level2Effect);
        _levelUPEffectsList.Add(Level3Effect);
        _levelUPEffectsList.Add(Level4Effect);
        _levelUPEffectsList.Add(Level5Effect);
        _levelUPEffectsList.Add(Level6Effect);
        _levelUPEffectsList.Add(Level7Effect);

    }

    protected virtual void ApplyStats()
    {
        _currentDamage = RoundValue(ItemSO.BaseDamage * ((ItemData.DamageMultiplier *  PlayerScript.Instance.CurrentAttackMultiplier) + _localDamageBuff));
        _currentSizeScale = _localSizeBuff + ItemData.Area * PlayerScript.Instance.CurrentAttackSizeBuff;
        _currentSpeed = ItemData.Speed + _localSpeedBuff;
        _currentPierce = ItemData.Pierce + _additionalPierce;
        _currentDeathtime = ItemData.Deathtime + _localDeathTimebuff;

        float CooldownBuff = 1f;
        if(ItemData.WeaponType == WeaponType.MELEE)
        {
            CooldownBuff = PlayerScript.Instance.MeleeCooldownBuff;
        }else if(ItemData.WeaponType == WeaponType.RANGED)
        {
            CooldownBuff = PlayerScript.Instance.RangedCooldownBuff;
        }

        _cooldownWaitUntilProjectileDeath = ItemData.CooldownWaitUntilProjectileDeath;
        _weaponDestroyedAfterPierce = ItemData.WeaponDestroyedAfterPierce;


        _currentCooldownDuration = ItemData.CooldownDuration * _localCooldownBuff * PlayerScript.Instance.CurrentHasteMultiplier * CooldownBuff;

        int RangedProjectileBuff = ItemData.WeaponType == WeaponType.RANGED ? PlayerScript.Instance.RangedProjectileBuff : 0;
        _projectileNum = ItemData.ProjectileNum + _additionalProjectiles + RangedProjectileBuff;

        _currentKnockbackValue = (ItemData.KnockbackValue + _additionalKnockbackValue) * PlayerScript.Instance.CurrentKnockbackBuff;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        _currentCooldown -= Time.deltaTime;
        if(_currentCooldown <= 0f && !_hasLaunchedSinceReset)
        {
            _hasLaunchedSinceReset = true;
            _currentCooldown = _currentCooldownDuration;
            Launch();
        }
    }

    protected virtual void Launch()
    {


        if (!_cooldownWaitUntilProjectileDeath)
            ResetCooldown();
    }

    public void ResetCooldown()
    {
        //Debug.Log($"{ItemData.name} Reset");
        _currentCooldown = _currentCooldownDuration;
        _hasLaunchedSinceReset = false;
    }

    public virtual void LevelUp()
    {
        _currentLevel++;
        _levelUPEffectsList[_currentLevel - 1]();
        CheckAttackRound();
        ApplyStats();
        LevelUpEffect();
    }

    protected virtual void LevelUpEffect()
    {

    }

    protected virtual void OnDestroy()
    {
        PlayerScript.Instance.onStatChange.RemoveListener(ApplyStats);
    }

    protected virtual void Level1Effect()
    {

    }
    protected virtual void Level2Effect()
    {
    }
    protected virtual void Level3Effect()
    {
    }
    protected virtual void Level4Effect()
    {
    }
    protected virtual void Level5Effect()
    {
    }
    protected virtual void Level6Effect()
    {
    }
    protected virtual void Level7Effect()
    {
    }

    //math

    public void CheckAttackRound()
    {
        if(_currentDamage % 1 != 0)
        {
            _currentDamage = RoundValue(_currentDamage);
        }
    }

    public static float RoundValue(float value)
    {
        return (float)Math.Round(value,0);
    }

    //General stat increases

    public void IncreaseDamagePercentage(float amount)
    {
        _localDamageBuff += amount;
    }

    public void IncreaseSpeedPercentage(float amount)
    {
        _localSpeedBuff += amount;
    }

    public void IncreaseSizePercentage(float amount)
    {
        _localSizeBuff += amount;
    }

    public void DecreaseCooldownPercentage(float amount)
    {
        _localCooldownBuff *= 1f - amount;
    }

    public void IncreaseDeathTime(float amount)
    {
        _localDeathTimebuff += amount;
    }


    //public void AddPierceLimit(int amount)
    //{
    //    _additionalPierce *= 2;
    //}

    public void AddPierceLimit(int amount)
    {
        _additionalPierce += amount;
    }

    public void IncreaseKnockBack(float amount)
    {
        _additionalKnockbackValue += amount;
    }

    public void UpdateBuff()
    {
        PlayerScript.Instance.UpdateBuffDictionary(this, _buff);
        BuffUIScript.Instance.UpdateBuff(ItemData, _buffStack);
    }

    protected ItemBehaviour InstantiateProjectile(bool independent = false)
    {
        if (ItemData.ProjectileSound != null)
        {
            SoundFXManager.Instance.PlaySoundFXClip(ItemData.ProjectileSound, transform, ItemData.Volume);
        }
        else
        {
            SoundFXManager.Instance.PlayBasicWhoosh(transform, 1f);
        }

        GameObject projectile;

        if (independent)
        {
            projectile = Instantiate(ItemData.ProjectileItemPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            projectile = Instantiate(ItemData.ProjectileItemPrefab, transform);
        }

        ItemBehaviour projectileBehaviour = projectile.GetComponent<ItemBehaviour>();

        if (projectileBehaviour != null)
        {
            projectileBehaviour.InitializeValue(this, _currentDamage, _currentDeathtime, _currentPierce, _currentSpeed, CurrentLevel, _currentSizeScale, _currentKnockbackValue, _stunTime);
            if (_cooldownWaitUntilProjectileDeath)
                projectileBehaviour.CooldownWaitUntilprojectileDeath = true;
            if (_weaponDestroyedAfterPierce)
                projectileBehaviour.WeaponDestroyedAfterPierce = true;

            CurrentProjectiles.Add(projectileBehaviour);

        }

        return projectileBehaviour;
    }

    protected void ResetProjectiles()
    {
        foreach(ItemBehaviour projectile in CurrentProjectiles)
        {
            if(projectile != null)
                projectile.DestroyProjectilesNow();

        }

        CurrentProjectiles.Clear();

        Launch();
    }

}
