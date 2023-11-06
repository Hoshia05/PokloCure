using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Item and Skill Data", menuName = "Scriptable Object/Item and Skill Data")]

public class ItemSkillSO : ScriptableObject
{
    public string ItemName;
    public ItemSkillType Type;
    public string ItemDescription;
    public Sprite ItemIcon;

    public GameObject ItemPrefab;
    public WeaponProfile WeaponProfile;


}