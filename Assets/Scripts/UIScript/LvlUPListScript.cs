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
            //���� ����Ʈ�� �� ����Ʈ�� ������(= ������ ��� �������� ���ܵǸ�) �׳� �׸� �����
            //���� ���ľ���
            //if (!GameManager.Instance.ItemList.Except(exemptList).Any())
            if(GameManager.Instance.ItemList.Count == exemptList.Count)
                break;

            GameObject ListObject = Instantiate(_lvlUpChoicePrefab, _itemListBase.transform);
            LvlUpChoiceScript lvlUpChoiceScript = ListObject.GetComponent<LvlUpChoiceScript>();

            ItemSO itemSkillSO;
            int nextLevel;

            //if item is already max level, reroll until different one shows up.
            do
            {
                //Get Random item
                itemSkillSO = GameManager.Instance.GetRandomItem();

                //Check if player already has it.
                nextLevel = PlayerScript.Instance.CheckItemPossessionLevel(itemSkillSO);

                if (nextLevel > itemSkillSO.ItemMaxLevel)
                    exemptList.Add(itemSkillSO);

            } while (exemptList.Contains(itemSkillSO));

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
