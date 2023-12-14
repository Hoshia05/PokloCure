using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    public static HPBar Instance;

    private Slider _hpSlider;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;

        _hpSlider = GetComponent<Slider>();

        _hpSlider.minValue = 0;

        PlayerScript.Instance.onMaxHPChange.AddListener(UpdateMaxHP);
    }

    public void InitializeHPBar(float initialHP)
    {
        _hpSlider.maxValue = initialHP;
        _hpSlider.value = initialHP;
        UIHPBar.Instance.InitializeHPBar(initialHP);
    }

    public void UpdateHP(float HPValue)
    {
        _hpSlider.value = HPValue;
        UIHPBar.Instance.UpdateHP(HPValue);
    }

    public void UpdateMaxHP(float newMaxHP)
    {
        _hpSlider.maxValue = newMaxHP;
        UIHPBar.Instance.UpdateMaxHP(newMaxHP);
    }

}
