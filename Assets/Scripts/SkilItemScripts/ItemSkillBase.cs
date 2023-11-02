using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSkillBase : MonoBehaviour
{
    [SerializeField]
    private string _itemName;
    [SerializeField]
    private ItemSkillType _type;
    [SerializeField]
    private string _itemDescription;
    [SerializeField]
    private Sprite _itemIcon;
}

public enum ItemSkillType
{
    SKILL,
    WEAPON,
    ITEM
}