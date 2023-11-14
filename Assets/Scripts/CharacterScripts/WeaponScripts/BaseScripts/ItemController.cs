using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour
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

    public int CurrentWeaponLevel
    {
        get { return _currentWeaponLevel; }
        set { _currentWeaponLevel = value; }
    }

    // Start is called before the first frame update
    protected virtual void Awake()
    {
        _currentDamage = ItemData.Damage;
        _currentSpeed = ItemData.Speed;
        _currentCooldownDuration = ItemData.CooldownDuration;
        _currentPierce = ItemData.Pierce;
        _currentDeathtime = ItemData.Deathtime;

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

    public virtual void LevelUp()
    {
        _currentWeaponLevel++;
        _levelUPEffectsList[_currentWeaponLevel - 1]();
    }

    //protected virtual void LevelEffectCheck()
    //{
    //    for (int i = _currentWeaponLevel - 1; i < _levelUPEffectsList.Count; i++)
    //    {
    //        _levelUPEffectsList[i]();
    //    }
    //}

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
}
