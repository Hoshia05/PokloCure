using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LvlUpChoiceScript : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _itemName;
    [SerializeField]
    private TextMeshProUGUI _type;
    [SerializeField]
    private TextMeshProUGUI _itemDescription;
    [SerializeField]
    private Image _itemIcon;

    private ItemSO _itemSkillData;

    private LvlUPListScript _parentList;


    public void InitializeWithData(ItemSO ISData, int NextLevel, LvlUPListScript ParentList)
    {
        _itemSkillData = ISData;

        string nextLevelText = NextLevel == ISData.ItemMaxLevel ? "MAX" : NextLevel.ToString();

        _itemName.text = $"{ISData.ItemName}  Lv.{nextLevelText}";
        _itemDescription.text = ISData.ItemDescription[NextLevel-1];
        _itemIcon.sprite = ISData.ItemImage;
        _type.text = ISData.ItemType.ToString();
        _parentList = ParentList;
    }

    public void SelectThisChoice()
    {
        StageManager.instance.GivePlayerItem(_itemSkillData);
        StageManager.instance.LevelUpEventEnd();
        _parentList.DeleteList();
    }

}
