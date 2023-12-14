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

    protected int _projectileNum = 1;

    protected delegate void LevelUPEffects();
    protected List<LevelUPEffects> _levelUPEffectsList;

    protected BuffObject _buff = new();

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
        _currentDamage = RoundValue(ItemSO.BaseDamage * ItemData.DamageMultiplier * PlayerScript.Instance.CurrentAttackMultiplier);
        _currentSpeed = ItemData.Speed;
        _currentCooldownDuration = ItemData.CooldownDuration * PlayerScript.Instance.CurrentHasteMultiplier;
        _currentPierce = ItemData.Pierce;
        _currentDeathtime = ItemData.Deathtime;
        _projectileNum = ItemData.ProjectileNum;
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
        _currentDamage *= 1f + amount;
    }

    public void IncreaseSizePercentage(float amount)
    {
        _currentSizeScale *= 1f + amount;
    }

    public void DecreaseCooldownPercentage(float amount)
    {
        _currentCooldownDuration *= 1f - amount;
    }

    public void IncreaseDeathTime(float amount)
    {
        _currentDeathtime += amount;
    }

    public void SetPierceLimit(int amount)
    {
        _currentPierce = amount;
    }

    public void UpdateBuff()
    {
        PlayerScript.Instance.UpdateBuffDictionary(this, _buff);
    }

}
