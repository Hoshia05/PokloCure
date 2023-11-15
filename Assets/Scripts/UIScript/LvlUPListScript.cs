using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LvlUPListScript : MonoBehaviour
{
    private const int _itemCount = 4;

    [SerializeField]
    private GameObject _lvlUpChoicePrefab;
    [SerializeField]
    private GameObject _itemListBase;

    private GameObject[] _itemList;

    private List<ItemSO> exemptList;

    public void CreateLvlUpList()
    {
        _itemList = new GameObject[_itemCount];
        exemptList = new();

        for (int i = 0; i < _itemCount; i++)
        {
            //제외 리스트가 총 리스트랑 같으면(= 가능한 모든 아이템이 제외되면) 그냥 그만 만들기
            if (!GameManager.Instance.ItemList.Except(exemptList).Any())
                break;

            GameObject ListObject = Instantiate(_lvlUpChoicePrefab, _itemListBase.transform);
            LvlUpChoiceScript lvlUpChoiceScript = ListObject.GetComponent<LvlUpChoiceScript>();

            //Get Random item
            ItemSO itemSkillSO = GameManager.Instance.GetRandomItem();

            //Check if player already has it.
            int nextLevel = PlayerScript.Instance.CheckItemPossessionLevel(itemSkillSO);

            //if item is already max level, reroll until different one shows up.
            while(nextLevel > itemSkillSO.ItemMaxLevel || exemptList.Contains(itemSkillSO))
            {
                itemSkillSO = GameManager.Instance.GetRandomItem();

                nextLevel = PlayerScript.Instance.CheckItemPossessionLevel(itemSkillSO);
            }

            lvlUpChoiceScript.InitializeWithData(itemSkillSO, nextLevel, this);
            _itemList[i] = ListObject;
            exemptList.Add(itemSkillSO);

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
