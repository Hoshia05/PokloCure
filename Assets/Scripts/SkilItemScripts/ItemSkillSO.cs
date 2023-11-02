using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Item/Skill Data", menuName = "Scriptable Object/Item/Skill Data")]

public class ItemSkillSO : ScriptableObject
{
    public string ItemName;
    public ItemSkillType Type;
    public string ItemDescription;
    public Sprite ItemIcon;

}