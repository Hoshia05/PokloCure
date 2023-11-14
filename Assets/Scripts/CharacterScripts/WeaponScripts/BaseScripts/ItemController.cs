using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    public ItemSO WeaponData;

    protected float _currentDamage;
    protected float _currentSpeed;
    protected float _currentCooldownDuration;
    protected int _currentPierce;

    protected float _currentCooldown;
    protected int _currentWeaponLevel = 1;

    protected delegate void LevelUPEffects();
    protected List<LevelUPEffects> _levelUPEffectsList;

    public int CurrentWeaponLevel
    {
        get { return _currentWeaponLevel; }
        set { _currentWeaponLevel = value; }
    }

    // Start is called before the first frame update
    protected virtual void Awake()
    {
        _currentDamage = WeaponData.Damage;
        _currentSpeed = WeaponData.Speed;
        _currentCooldownDuration = WeaponData.CooldownDuration;
        _currentPierce = WeaponData.Pierce;

        _currentCooldown = 0;

        _levelUPEffectsList = new List<LevelUPEffects>();
        _levelUPEffectsList.Add(Level2Effect);
        _levelUPEffectsList.Add(Level3Effect);
        _levelUPEffectsList.Add(Level4Effect);
        _levelUPEffectsList.Add(Level5Effect);
        _levelUPEffectsList.Add(Level6Effect);
        _levelUPEffectsList.Add(Level7Effect);
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        _currentCooldown -= Time.deltaTime;
        if(_currentCooldown <= 0f ) 
        {
            Attack();
        }
    }

    protected virtual void Attack()
    {
        _currentCooldown = _currentCooldownDuration;
    }

    protected virtual void LevelUp()
    {
        _currentWeaponLevel++;
    }

    protected virtual void LevelEffectCheck()
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
}
