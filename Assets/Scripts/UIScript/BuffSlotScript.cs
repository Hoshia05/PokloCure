using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuffSlotScript : MonoBehaviour
{
    [SerializeField] private Image _buffIcon;
    [SerializeField] private TextMeshProUGUI _bufflevel;

    private int _currentLevel = 1;

    public void IntitializeBuffIcon(Sprite Icon)
    {
        UpdateIcon(Icon);
        UpdateLevel(_currentLevel);
    }

    public void UpdateIcon(Sprite icon)
    {
        _buffIcon.sprite = icon;
    }

    public void UpdateLevel(int level)
    {
        _bufflevel.text = level.ToString();
    }

}
