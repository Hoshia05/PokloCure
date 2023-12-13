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


        //ItemSO itemSkillSO;
        //int nextLevel;


        //for (int i = 0; i < _itemMaxCount; i++)
        //{
        //    //Á¦¿Ü ¸®½ºÆ®°¡ ÃÑ ¸®½ºÆ®¶û °°À¸¸é(= °¡´ÉÇÑ ¸ðµç ¾ÆÀÌÅÛÀÌ Á¦¿ÜµÇ¸é) ±×³É ±×¸¸ ¸¸µé±â
        //    //¹ö±× °íÃÄ¾î¾î¤Ã
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

        //    //    //º¸À¯ÇÏ°í ÀÖÁö¸¸ ÀÌ¹Ì ¸¸·¾ »óÅÂ
        //    //    if(nextLevel > itemSkillSO.ItemMaxLevel)
        //    //    {
        //    //        exemptList.Add(itemSkillSO);
        //    //    }

        //    //    //ÀÌ¹Ì ¸ðµç ¾ÆÀÌÅÛÀÌ Á¦¿Ü¸®½ºÆ®¿¡ Æ÷ÇÔµÇ¾î ÀÖ¾î¼­ ´õÀÌ»ó ¸®½ºÆ®¿¡ ³ÖÀ» ¼ö ÀÖ´Â°Ô ¾ø´Â »óÅÂ
        //    //    if (exemptList.Count >= GameManager.Instance.ItemList.Count)
        //    //        break;

        //    //} 

        //    //ÀÌ¹Ì ¸ðµç ¾ÆÀÌÅÛÀÌ Á¦¿Ü¸®½ºÆ®¿¡ Æ÷ÇÔµÇ¾î ÀÖ¾î¼­ ´õÀÌ»ó ¸®½ºÆ®¿¡ ³ÖÀ» ¼ö ÀÖ´Â°Ô ¾ø´Â »óÅÂ
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
