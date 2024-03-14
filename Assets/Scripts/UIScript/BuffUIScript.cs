using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffUIScript : MonoBehaviour
{
    public static BuffUIScript Instance;

    [SerializeField] private GameObject _buffSlotPrefab;

    private Dictionary<string, BuffSlotScript> _buffDictionary = new();

    private void Awake()
    {
        Instance = this;
    }

    public void UpdateBuff(ItemSO buffItem, int StackValue = 1)
    {
        string buffName = buffItem.ItemName;

        //if StackValue == 0, delete
        if(StackValue == 0)
        {
            if (_buffDictionary.ContainsKey(buffName))
            {
                BuffSlotScript deleteBuff = _buffDictionary[buffName];
                GameObject buffSlot = deleteBuff.gameObject;
                Destroy(buffSlot);
                _buffDictionary.Remove(buffName);
            }
        }
        else
        {
            if (!_buffDictionary.ContainsKey(buffName))
            {
                BuffSlotScript newBuff = AddNewBuff(buffItem.ItemImage, StackValue);

                _buffDictionary.Add(buffName, newBuff);

            }
            else
            {
                BuffSlotScript currentBuff = _buffDictionary[buffName];
                currentBuff.UpdateLevel(StackValue);
            }
        }

    }

    public BuffSlotScript AddNewBuff(Sprite Icon, int stack = 1)
    {
        GameObject NewBuffSlot = Instantiate(_buffSlotPrefab, transform);
        BuffSlotScript bsScript = NewBuffSlot.GetComponent<BuffSlotScript>();
        bsScript.IntitializeBuffIcon(Icon);

        if(stack != 1)
        {
            bsScript.UpdateLevel(stack);
        }

        return bsScript;
    }

    public void AddBuffStack()
    {

    }

}
