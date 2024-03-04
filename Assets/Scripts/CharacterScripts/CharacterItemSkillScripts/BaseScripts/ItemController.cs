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
    protected int _currentWeaponLevel = 1;
    protected float _currentSizeScale = 1;

    protected float _currentKnockbackValue = 1;

    protected float _localDamageBuff = 0f;
    protected float _localSpeedBuff = 0f;
    protected float _localSizeBuff = 0f;
    protected float _localCooldownBuff = 1f;
    protected float _localDeathTimebuff = 0;

    protected int _projectileNum = 1;
    protected int _additionalProjectiles = 0;

    public List<ItemBehaviour> CurrentProjectiles = new();

    protected delegate void LevelUPEffects();
    protected List<LevelUPEffects> _levelUPEffectsList;

    protected BuffObject _buff = new();

    protected bool _deathTimeCoolTimeCumulative;
    protected bool _hasLaunchedSinceReset;

    public int CurrentWeaponLevel
    {
        get { return _currentWeaponLevel; }
        set { _currentWeaponLevel = value; }
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
        _currentPierce = ItemData.Pierce;
        _currentDeathtime = ItemData.Deathtime + _localDeathTimebuff;
        _currentCooldownDuration = _deathTimeCoolTimeCumulative ?
           _currentDeathtime + ItemData.CooldownDuration * _localCooldownBuff * PlayerScript.Instance.CurrentHasteMultiplier  : 
            ItemData.CooldownDuration * _localCooldownBuff * PlayerScript.Instance.CurrentHasteMultiplier;
        _projectileNum = ItemData.ProjectileNum + _additionalProjectiles;
        _currentKnockbackValue = ItemData.KnockbackValue * PlayerScript.Instance.CurrentKnockbackBuff;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        _currentCooldown -= Time.deltaTime;
        if(_currentCooldown <= 0f && !_hasLaunchedSinceReset)
        {
            _hasLaunchedSinceReset = true;
            Launch();
        }
    }

    protected virtual void Launch()
    {
        ResetCooldown();
    }

    public void ResetCooldown()
    {
        _currentCooldown = _currentCooldownDuration;
        _hasLaunchedSinceReset = false;
    }

    public virtual void LevelUp()
    {
        _currentWeaponLevel++;
        _levelUPEffectsList[_currentWeaponLevel - 1]();
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

    public void SetPierceLimit(int amount)
    {
        _currentPierce = amount;
    }

    public void UpdateBuff()
    {
        PlayerScript.Instance.UpdateBuffDictionary(this, _buff);
    }

    protected ItemBehaviour InstantiateProjectile()
    {
        GameObject projectile = Instantiate(ItemData.ProjectileItemPrefab, transform);
        ItemBehaviour projectileBehaviour = projectile.GetComponent<ItemBehaviour>();
        projectileBehaviour.InitializeValue(this, _currentDamage, _currentDeathtime, _currentPierce, _currentSpeed, CurrentWeaponLevel, _currentSizeScale, _currentKnockbackValue);

        CurrentProjectiles.Add(projectileBehaviour);

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
