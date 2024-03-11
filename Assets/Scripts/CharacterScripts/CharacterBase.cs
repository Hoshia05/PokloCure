using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Unassigned Character Data", menuName = "Scriptable Object/Character Data")]
public class CharacterBase : ScriptableObject
{
    public const float _baseSpeed = 7f;
    public const float _baseCooltime = 0.5f;
    public const float _baseCriticalChance = 0.01f;
    public const float _baseCritDamage = 2f;
    public const float _baseItemEatDistance = 5f;
    public const float _baseHaste = 1f;

    public Sprite CharacterPortrait;
    public Sprite CharacterSprite;
    public CharacterDistinct characterLabel = CharacterDistinct.NONE;
    

    public RuntimeAnimatorController AnimatorController;
    public ItemSO BaseWeapon;

    public string CharacterName;
    public string CharacterDescription;

    public float Health = 100f;
    public float Stamina = 100f;
    public float DashMultiplier = 2f;
    public float SpeedMultiplier = 1f;
    public float AttackMultiplier = 1f;
    public float CriticalMultiplier = 0.01f;

}
