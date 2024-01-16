﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LvlUPListScript : MonoBehaviour
{
    private const int _itemMaxCount = 4;

    [SerializeField]
    private GameObject _lvlUpChoicePrefab;
    [SerializeField]
    private GameObject _itemListBase;

    private GameObject[] _itemList;

    private List<ItemSO> exemptList;

    public void CreateLvlUpList()
    {
        _itemList = new GameObject[_itemMaxCount];
        exemptList = new List<ItemSO>(StageManager.instance.ItemExemptList);

        int count = 0;
        
        while(count < _itemMaxCount)
        {
            ItemSO itemSkillSO = GameManager.Instance.GetRandomItem(exemptList);

            if(itemSkillSO == null)
            {
                break;
            }

            int nextLevel = PlayerScript.Instance.CheckItemPossessionLevel(itemSkillSO);

            //이미 만렙인 경우
            if (nextLevel > itemSkillSO.ItemMaxLevel)
            {
                exemptList.Add(itemSkillSO);
                continue;
            }

            GameObject ListObject = Instantiate(_lvlUpChoicePrefab, _itemListBase.transform);
            LvlUpChoiceScript lvlUpChoiceScript = ListObject.GetComponent<LvlUpChoiceScript>();

            lvlUpChoiceScript.InitializeWithData(itemSkillSO, nextLevel, this);
            _itemList[count] = ListObject;
            exemptList.Add(itemSkillSO);
            count++;
        }

        exemptList = new();

    }

    public void DeleteList()
    {
        foreach(GameObject ListObject in _itemList)
        {
            Destroy(ListObject);
        }
    }

}
