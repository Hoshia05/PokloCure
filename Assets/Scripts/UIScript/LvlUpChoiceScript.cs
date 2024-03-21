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
    private GameObject _itemCostObject;
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

        if(_itemSkillData.Cost == 0)
        {
            _itemCostObject.SetActive(false);
        }
        else
        {
            TextMeshProUGUI itemCostText = _itemCostObject.GetComponentInChildren<TextMeshProUGUI>();
            itemCostText.text = _itemSkillData.Cost.ToString();
            itemCostText.color = _itemSkillData.Cost > StageManager.Instance.CoinCount ? Color.red : Color.white;
        }

        _itemIcon.sprite = ISData.ItemImage;
        _type.text = $">>{ISData.ItemType.ToString()}";
        _parentList = ParentList;
    }

    public void SelectThisChoice()
    {
        //CheckCost
        if (_itemSkillData.Cost > 0)
        {
            if(_itemSkillData.Cost > StageManager.Instance.CoinCount)
            {
                return;
            }
            else
            {
                StageManager.Instance.GainCoins(-_itemSkillData.Cost);
                StageManager.Instance.GivePlayerItem(_itemSkillData);
                StageManager.Instance.LevelUpEventEnd();
                _parentList.DeleteList();
            }
        }
        else
        {
            StageManager.Instance.GivePlayerItem(_itemSkillData);
            StageManager.Instance.LevelUpEventEnd();
            _parentList.DeleteList();
        }

    }

}
