using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BoxItemUIScript : MonoBehaviour
{

    private List<ItemSO> exemptList;

    private Dictionary<ItemSO, int> _pulledItemDict;

    private int ItemNum;

    [SerializeField]
    private List<GameObject> _boxes;

    [SerializeField]
    private List<GameObject> _itemPositions;

    [SerializeField]
    private GameObject _openButton;
    [SerializeField]
    private GameObject _takeButton;
    [SerializeField]
    private GameObject _dropButton;


    [SerializeField]
    private GameObject _descriptionWindow;
    [SerializeField]
    private TextMeshProUGUI _itemName;
    [SerializeField]
    private TextMeshProUGUI _itemDescription;
    [SerializeField]
    private TextMeshProUGUI _itemType;

    private int _itemIndex = 0;

    public void InitializeUI()
    {
        //ItemNum = GameManager.Instance.Rand.Next(1, 3);
        ItemNum = 1;


        _boxes.ForEach(x => x.SetActive(false));
        _itemPositions.ForEach(x => x.SetActive(false));

        for (int i = 0; i < ItemNum; i++)
        {
            _boxes[i].SetActive(true);
        }

        _descriptionWindow.SetActive(false);
        _openButton.SetActive(true);
        _takeButton.SetActive(false);
        _dropButton.SetActive(false);

        _pulledItemDict = new();
    }

    public void PullRandomItem()
    {
        _itemIndex = 0;
        _pulledItemDict = new();
        exemptList = new List<ItemSO>(StageManager.Instance.ItemExemptList);

        int count = 0;

        while (count < ItemNum)
        {
            //stage manager랑 exemptList 체크하는게 중복됨. 사용하는데 문제는 없는데 나중에 클린업 하자...

            ItemSO itemSkillSO = GameManager.Instance.GetRandomItem(exemptList);

            if (itemSkillSO == null)
            {
                break;
            }

            int nextLevel = PlayerScript.Instance.CheckItemPossessionLevel(itemSkillSO);

            //if (nextLevel > itemSkillSO.ItemMaxLevel)
            //{
            //    exemptList.Add(itemSkillSO);
            //    continue;
            //}

            _pulledItemDict.Add(itemSkillSO, nextLevel);
            exemptList.Add(itemSkillSO);

            _itemPositions[count].SetActive(true);


            Image ItemImage = _itemPositions[count].GetComponent<Image>();
            ItemImage.sprite = itemSkillSO.ItemImage;

            count++;
        }

        _descriptionWindow.SetActive(true);
        _openButton.SetActive(false);
        _takeButton.SetActive(true);
        _dropButton.SetActive(true);

        UpdateDescription(_pulledItemDict.ElementAt(_itemIndex));

    }

    public void UpdateDescription(KeyValuePair<ItemSO, int> itemData)
    {
        ItemSO ItemData = itemData.Key;
        int NextLevel = itemData.Value;

        _itemName.text = $"{ItemData.ItemName} Lv.{NextLevel.ToString()}";
        _itemDescription.text = ItemData.ItemDescription[NextLevel-1];
        _itemType.text = $">> {ItemData.ItemType}";
    }

    public void TakeButtonClick()
    {
        StageManager.Instance.GivePlayerItem(_pulledItemDict.ElementAt(_itemIndex).Key);
        CheckNext();
    }

    public void DropButtonClick()
    {
        CheckNext();
    }

    public void CheckNext()
    {
        _itemIndex++;
        if(_pulledItemDict.Count <= _itemIndex)
        {
            StageManager.Instance.EndBoxUI();
        }
        else
        {
            UpdateDescription(_pulledItemDict.ElementAt(_itemIndex));
        }
    }

}
