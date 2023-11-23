using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BoxItemUIScript : MonoBehaviour
{

    private List<ItemSO> exemptList;

    private Dictionary<ItemSO, int> _pulledItemDict;

    private int ItemNum;

    [SerializeField]
    private List<GameObject> _boxes;

    [SerializeField]
    private List<Image> _itemPositions;

    public void InitializeUI()
    {
        ItemNum = GameManager.Instance.Rand.Next(1, 3);

        _boxes.ForEach(x => x.SetActive(false));
        _itemPositions.ForEach(x => x.sprite = null);

        for(int i = 0; i < ItemNum; i++)
        {
            _boxes[i].SetActive(true);
        }

        _pulledItemDict = new();
    }

    public void PullRandomItem()
    {
        exemptList = new();

        ItemSO itemSkillSO;
        int nextLevel;

        for(int i = 0; i < ItemNum; i++)
        {
            do
            {
                //Get Random item
                itemSkillSO = GameManager.Instance.GetRandomItem();

                //Check if player already has it.
                nextLevel = PlayerScript.Instance.CheckItemPossessionLevel(itemSkillSO);

                if (nextLevel > itemSkillSO.ItemMaxLevel)
                    exemptList.Add(itemSkillSO);

            } while (exemptList.Contains(itemSkillSO));

            _pulledItemDict.Add(itemSkillSO, nextLevel);

            _itemPositions[i].sprite = itemSkillSO.ItemImage;
        }

    }
}
