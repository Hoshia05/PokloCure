using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LvlUPListScript : MonoBehaviour
{
    private const int _itemCount = 4;

    [SerializeField]
    private GameObject _lvlUpChoicePrefab;
    [SerializeField]
    private GameObject _itemListBase;

    private GameObject[] _itemList;

    public void CreateLvlUpList()
    {
        _itemList = new GameObject[_itemCount];

        for(int i = 0; i < _itemCount; i++)
        {
            GameObject ListObject = Instantiate(_lvlUpChoicePrefab, _itemListBase.transform);
            LvlUpChoiceScript lvlUpChoiceScript = ListObject.GetComponent<LvlUpChoiceScript>();

            //Get Random item
            ItemSO itemSkillSO = GameManager.Instance.GetRandomItem();

            //Check if player already has it.
            int nextLevel = PlayerScript.Instance.CheckItemPossessionLevel(itemSkillSO);

            //if item is already max level, reroll until different one shows up.
            while(nextLevel > itemSkillSO.ItemMaxLevel)
            {
                itemSkillSO = GameManager.Instance.GetRandomItem();

                nextLevel = PlayerScript.Instance.CheckItemPossessionLevel(itemSkillSO);
            }

            lvlUpChoiceScript.InitializeWithData(itemSkillSO, nextLevel, this);
            _itemList[i] = ListObject;

        }
    }

    public void DeleteList()
    {
        foreach(GameObject ListObject in _itemList)
        {
            Destroy(ListObject);
        }
    }

}
