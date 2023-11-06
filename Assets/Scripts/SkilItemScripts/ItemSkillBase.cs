using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSkillBase
{
    public string ItemName;
    public ItemSkillType Type;
    public string ItemDescription;
    public Sprite ItemIcon;

    public ItemSkillBase(ItemSkillSO scriptableObject)
    {
        InitializeWithSO(scriptableObject);
    }

    public void InitializeWithSO(ItemSkillSO scriptableObject)
    {
        ItemName = scriptableObject.name;
        Type = scriptableObject.Type;
        ItemDescription = scriptableObject.ItemDescription;
        ItemIcon = scriptableObject.ItemIcon;
    }
}

public enum ItemSkillType
{
    SKILL,
    WEAPON,
    ITEM
}