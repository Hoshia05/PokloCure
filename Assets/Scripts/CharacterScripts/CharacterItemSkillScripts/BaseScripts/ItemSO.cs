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

    public System.Type ControllerType => System.Type.GetType(ControllerScript.name.ToString());

    [SerializeField]
    private int _cost = 0;

    [SerializeField]
    private int _costMultiplier = 1;

    public float DamageMultiplier = 1f;
    public float Speed;
    public float CooldownDuration = 1f;
    public int Pierce = 1;
    public float Deathtime = 4f;
    public int ProjectileNum = 1;
    public float KnockbackValue = 0f;
    public float Area = 1f;
    public int Priority = 4;

    public bool CooldownWaitUntilProjectileDeath = false;

    public int GetCost(int currentLevel)
    {
        return _cost * currentLevel;
    }

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
