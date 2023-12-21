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
    public Sprite ItemImage;
    public int ItemMaxLevel = 7;
    public CharacterDistinct characterLabel = CharacterDistinct.NONE;

    //public GameObject ControllerPrefab;
    public GameObject ProjectileItemPrefab;

    public Object ControllerScript;

    public float DamageMultiplier = 1f;
    public float Speed;
    public float CooldownDuration;
    public int Pierce = 1;
    public float Deathtime = 4f;
    public int ProjectileNum = 1;
    public float KnockbackValue = 0f;
    public float Area = 1f;
}

public enum ItemType
{
    ITEM,
    WEAPON,
    SKILL
}

public enum CharacterDistinct
{
    NONE,
    YEOMROCK,
}
