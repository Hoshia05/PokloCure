using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Character Data", menuName = "Scriptable Object/Character Data")]
public class CharacterBase : ScriptableObject
{
    public const float _baseSpeed = 10f;
    public const float _baseCooltime = 0.5f;
    public const float _baseCritical = 0.1f;
    public const float _baseItemEatDistance = 5f;

    public Sprite CharacterPortrait;
    public Sprite CharacterSprite;

    public RuntimeAnimatorController AnimatorController;

    public string CharacterName;

    public float Health = 100f;
    public float SpeedMultiplier = 1f;
    public float AttackMultiplier = 1f;
    public float CriticalChance = 0.1f;
    public float DefensePoints = 5f;

}
