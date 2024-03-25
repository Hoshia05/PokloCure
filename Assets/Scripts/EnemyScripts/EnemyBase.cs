using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Unassigned Enemy Data", menuName = "Scriptable Object/Enemy Data")]
public class EnemyBase : ScriptableObject
{
    public const float _baseSpeed = 2f;
    public const float _baseCooltime = 0.5f;
    public const float _baseCriticalChance = 0.01f;
    public const float _baseCritDamage = 2f;
    public const float _baseItemEatDistance = 5f;

    public Sprite EnemyPortrait;
    public Sprite EnemySprite;

    public RuntimeAnimatorController AnimatorController;
    public GameObject BasicWeaponController;

    public string EnemyName;
    public string EnemyDescription;

    public bool isBossEnemy = false;
    public EnemyType EnemyType = EnemyType.MELEE;
    public EnemyClass EnemyClass = EnemyClass.SWARM;
    public float HP = 10f;
    public float SpeedMultiplier = 1f;
    public float BodyDamage = 10f;
    public float AttackCooltime = 0.5f;
    public float CriticalChance = 0f;
    public float DropEXP;
    public float SizeScale = 1f;

    //RangedAttackRelated

    public GameObject RangedAttackProjectilePrefab;
    public float _projectileNum = 0;

    [SerializeField]
    public float _rangedAttackDamage = 0;
    public float RangedAttackDistance = 0;
    public float RangedAttackCooltime = 0;


    public int EnemyScore = 10;
}

public enum EnemyType
{
    MELEE,
    RANGED,
    HYBRID,
    BOSS,
}

public enum EnemyClass
{
    SWARM,
    MEDIUM,
    ELITE,
}
