using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ItemSO", menuName = "Scriptable Object/ItemSO")]
public class ItemSO : ScriptableObject
{
    public string ItemName;
    public string ItemDescription;
    public ItemType ItemType = ItemType.WEAPON;
    public Sprite ItemImage;
    public int ItemMaxLevel = 7;

    public GameObject ControllerPrefab;
    public GameObject ItemPrefab;
    public float Damage;
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
