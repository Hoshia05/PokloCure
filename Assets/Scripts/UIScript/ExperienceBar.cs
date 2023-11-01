using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExperienceBar : MonoBehaviour
{
    public static ExperienceBar Instance;

    private Slider _expSlider;
    [SerializeField]
    private TextMeshProUGUI _lvlText;

    private int _currentlvl = 1;

    private void Awake()
    {
        Instance = this;

        _expSlider = GetComponent<Slider>();
        SetLvlText();

    }

    private void SetLvlText()
    {
        _lvlText.text = $"Lvl: {_currentlvl}";
    }

    public void InitializeEXP(float initialMaxValue)
    {
        _expSlider.minValue = 0;
        _expSlider.maxValue = initialMaxValue;
        _expSlider.value = 0;

    }

    public void LvlUp(int newCurrentlvl, float nextLvlUp)
    {
        _currentlvl = newCurrentlvl;
        _expSlider.maxValue = nextLvlUp;
        SetLvlText();
    }

    public void SetEXP(float expValue, int currentLvl)
    {
        _expSlider.value = expValue;
    }

}
