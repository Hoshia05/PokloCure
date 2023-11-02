using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LvlUpChoiceScript : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _itemName;
    [SerializeField]
    private TextMeshProUGUI _type;
    [SerializeField]
    private TextMeshProUGUI _itemDescription;
    [SerializeField]
    private Image _itemIcon;

    public void InitializeWithSO(ItemSkillSO ISData)
    {
        _itemName.text = ISData.ItemName;
        _itemDescription.text = ISData.ItemDescription;
        _itemIcon.sprite = ISData.ItemIcon;
        _type.text = ISData.Type.ToString();
    }

}
