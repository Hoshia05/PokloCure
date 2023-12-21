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

    protected float _localDamageBuff = 1f;
    protected float _localSpeedBuff = 1f;
    protected float _localSizeBuff = 1f;
    protected float _localCooldownBuff = 1f;
    protected float _localDeathTimebuff = 0;

    protected int _projectileNum = 1;
    protected int _additionalProjectiles = 0;

    protected List<GameObject> _currentProjectiles = new();

    protected delegate void LevelUPEffects();
    protected List<LevelUPEffects> _levelUPEffectsList;

    protected BuffObject _buff = new();

    protected bool _deathTimeCoolTimeCumulative;

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

    private void ApplyStats()
    {
        _currentDamage = RoundValue(ItemSO.BaseDamage * ItemData.DamageMultiplier * _localDamageBuff  *  PlayerScript.Instance.CurrentAttackMultiplier);
        _currentSizeScale = _localSizeBuff * ItemData.Area * PlayerScript.Instance.CurrentAttackSizeBuff;
        _currentSpeed = ItemData.Speed;
        _currentPierce = ItemData.Pierce;
        _currentDeathtime = ItemData.Deathtime + _localDeathTimebuff;
        _currentCooldownDuration = _deathTimeCoolTimeCumulative ? 
            ItemData.Deathtime + _localDeathTimebuff + ItemData.CooldownDuration * _localCooldownBuff * PlayerScript.Instance.CurrentHasteMultiplier  : 
            ItemData.CooldownDuration * _localCooldownBuff * PlayerScript.Instance.CurrentHasteMultiplier;
        _projectileNum = ItemData.ProjectileNum + _additionalProjectiles;
        _currentKnockbackValue = ItemData.KnockbackValue * PlayerScript.Instance.CurrentKnockbackBuff;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        _currentCooldown -= Time.deltaTime;
        if(_currentCooldown <= 0f ) 
        {
            Launch();
        }
    }

    protected virtual void Launch()
    {
        _currentCooldown = _currentCooldownDuration;
    }

    public virtual void LevelUp()
    {
        _currentWeaponLevel++;
        _levelUPEffectsList[_currentWeaponLevel - 1]();
        CheckAttackRound();
        LevelUpEffect();
        ApplyStats();
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
        _localDamageBuff *= 1f + amount;
    }

    public void IncreaseSizePercentage(float amount)
    {
        _localSizeBuff *= 1f + amount;
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
        projectileBehaviour.InitializeValue(_currentDamage, _currentDeathtime, _currentPierce, _currentSpeed, CurrentWeaponLevel, _currentSizeScale, _currentKnockbackValue);

        _currentProjectiles.Add(projectile);

        return projectileBehaviour;
    }

    protected void ResetProjectiles()
    {
        foreach(GameObject projectile in _currentProjectiles)
        {
            Destroy(projectile);
        }

        Launch();
    }

}
