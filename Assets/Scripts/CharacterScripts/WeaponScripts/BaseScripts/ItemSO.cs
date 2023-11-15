using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CreateAssetMenu(fileName = "ItemSO", menuName = "Scriptable Object/ItemSO")]
public class ItemSO : ScriptableObject
{
    public const float BaseDamage = 10f;

    public string ItemName;
    public List<string> ItemDescription;
    public ItemType ItemType = ItemType.WEAPON;
    public Sprite ItemImage;
    public int ItemMaxLevel = 7;

    public GameObject ControllerPrefab;
    public GameObject ItemPrefab;
    public float DamageMultiplier = 1f;
    public float Speed;
    public float CooldownDuration;
    public int Pierce = 1;
    public float Deathtime = 4f;
}

public enum ItemType
{
    ITEM,
    WEAPON,
    SKILL
}
