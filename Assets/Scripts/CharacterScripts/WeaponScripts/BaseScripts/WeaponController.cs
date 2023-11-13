using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("Weapon Stats")]

    public WeaponSO WeaponData;

    //public GameObject Prefab;
    //public float Damage;
    //public float Speed;
    //public float CooldownDuration;
    //public int Pierce = 1;

    protected float _currentDamage;
    protected float _currentSpeed;
    protected float _currentCooldownDuration;
    protected int _currentPierce;

    protected float Deathtime = 4f;

    private float _currentCooldown;
    private int _currentWeaponLevel = 1;

    // Start is called before the first frame update
    protected virtual void Awake()
    {
        _currentDamage = WeaponData.Damage;
        _currentSpeed = WeaponData.Speed;
        _currentCooldownDuration = WeaponData.CooldownDuration;
        _currentPierce = WeaponData.Pierce;

        _currentCooldown = _currentCooldownDuration;

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
}
