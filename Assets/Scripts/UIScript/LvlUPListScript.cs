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


    [Header("������")]
    [SerializeField]
    private ItemSkillSO testSO;

    public void CreateLvlUpList()
    {
        _itemList = new GameObject[_itemCount];

        for(int i = 0; i < _itemCount; i++)
        {
            GameObject ListObject = Instantiate(_lvlUpChoicePrefab, _itemListBase.transform);
            LvlUpChoiceScript lvlUpChoiceScript = ListObject.GetComponent<LvlUpChoiceScript>();

            ItemSkillSO itemSkillSO = testSO; //get random
            ItemSkillBase itemSkillData = new(itemSkillSO);

            lvlUpChoiceScript.InitializeWithData(itemSkillData);

        }
    }

}
