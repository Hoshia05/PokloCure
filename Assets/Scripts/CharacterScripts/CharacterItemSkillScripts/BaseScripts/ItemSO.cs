using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "ItemSO", menuName = "Scriptable Object/ItemSO")]
public class ItemSO : ScriptableObject
{
    public const float BaseDamage = 10f;

    public string ItemName;
    public List<string> ItemDescription;
    public ItemType ItemType = ItemType.WEAPON;
    public WeaponType WeaponType = WeaponType.NONE;
    public Sprite ItemImage;
    public int ItemMaxLevel = 5;
    public CharacterDistinct characterLabel = CharacterDistinct.NONE;

    //public GameObject ControllerPrefab;
    public GameObject ProjectileItemPrefab;

    public Object ControllerScript;

    public int Cost = 0;
    public float DamageMultiplier = 1f;
    public float Speed;
    public float CooldownDuration = 1f;
    public int Pierce = 1;
    public float Deathtime = 4f;
    public int ProjectileNum = 1;
    public float KnockbackValue = 0f;
    public float Area = 1f;
    public int Priority = 4;
}

public enum ItemType
{
    ITEM,
    WEAPON,
    SKILL,
    UPGRADE,
}

public enum WeaponType
{
    MELEE,
    RANGED,
    NONE,
}

public enum CharacterDistinct
{
    NONE,
    YEOMROCK,
    BLUETIGER,
    GREENTIGER,
}
