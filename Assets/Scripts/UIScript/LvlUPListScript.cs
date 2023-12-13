using System.Collections;
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
        exemptList = StageManager.instance.ItemExemptList;

        int count = 0;
        
        while(count < _itemMaxCount)
        {
            ItemSO itemSkillSO = GameManager.Instance.GetRandomItem(exemptList);

            if(itemSkillSO == null)
            {
                break;
            }

            int nextLevel = PlayerScript.Instance.CheckItemPossessionLevel(itemSkillSO);

            //벌써 만렙이라면
            if (nextLevel > itemSkillSO.ItemMaxLevel)
            {
                exemptList.Add(itemSkillSO);
                continue;
            }

            GameObject ListObject = Instantiate(_lvlUpChoicePrefab, _itemListBase.transform);
            LvlUpChoiceScript lvlUpChoiceScript = ListObject.GetComponent<LvlUpChoiceScript>();

            lvlUpChoiceScript.InitializeWithData(itemSkillSO, nextLevel, this);
            _itemList[count] = ListObject;
            count++;
        }



        //ItemSO itemSkillSO;
        //int nextLevel;


        //for (int i = 0; i < _itemMaxCount; i++)
        //{
        //    //제외 리스트가 총 리스트랑 같으면(= 가능한 모든 아이템이 제외되면) 그냥 그만 만들기
        //    //버그 고쳐어어ㅓ
        //    //if (!GameManager.Instance.ItemList.Except(exemptList).Any())

        //    itemSkillSO = GameManager.Instance.GetRandomItem(exemptList);

        //    //Check if player already has it.
        //    nextLevel = PlayerScript.Instance.CheckItemPossessionLevel(itemSkillSO);

        //    if (nextLevel > itemSkillSO.ItemMaxLevel)
        //    {
        //        exemptList.Add(itemSkillSO);
        //    }

        //    //if item is already max level, reroll until different one shows up.
        //    //while (exemptList.Contains(itemSkillSO))
        //    //{
        //    //    //Get Random item
        //    //    itemSkillSO = GameManager.Instance.GetRandomItem();

        //    //    //Check if player already has it.
        //    //    nextLevel = PlayerScript.Instance.CheckItemPossessionLevel(itemSkillSO);

        //    //    //보유하고 있지만 이미 만렙 상태
        //    //    if(nextLevel > itemSkillSO.ItemMaxLevel)
        //    //    {
        //    //        exemptList.Add(itemSkillSO);
        //    //    }

        //    //    //이미 모든 아이템이 제외리스트에 포함되어 있어서 더이상 리스트에 넣을 수 있는게 없는 상태
        //    //    if (exemptList.Count >= GameManager.Instance.ItemList.Count)
        //    //        break;

        //    //} 

        //    //이미 모든 아이템이 제외리스트에 포함되어 있어서 더이상 리스트에 넣을 수 있는게 없는 상태
        //    //if (exemptList.Count >= GameManager.Instance.ItemList.Count)
        //    //    break;

        //    GameObject ListObject = Instantiate(_lvlUpChoicePrefab, _itemListBase.transform);
        //    LvlUpChoiceScript lvlUpChoiceScript = ListObject.GetComponent<LvlUpChoiceScript>();

        //    lvlUpChoiceScript.InitializeWithData(itemSkillSO, nextLevel, this);
        //    _itemList[i] = ListObject;
        //    exemptList.Add(itemSkillSO);

        //}
    }

    public void DeleteList()
    {
        foreach(GameObject ListObject in _itemList)
        {
            Destroy(ListObject);
        }
    }

}
