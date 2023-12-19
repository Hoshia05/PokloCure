using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIHPBar : MonoBehaviour
{
    public static UIHPBar Instance;

    private Slider _hpSlider;
    [SerializeField]
    private TextMeshProUGUI _hpText;

    private LayoutElement _hpLayoutElement;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;

        _hpSlider = GetComponent<Slider>();
        _hpLayoutElement = GetComponent<LayoutElement>();

        _hpSlider.minValue = 0;
    }

    public void InitializeHPBar(float initialHP)
    {
        _hpSlider.maxValue = initialHP;
        _hpSlider.value = initialHP;
        _hpLayoutElement.flexibleWidth = initialHP;
        UpdateHPText();
    }

    // Update is called once per frame
    public void UpdateHP(float HPValue)
    {
        _hpSlider.value = HPValue;
        UpdateHPText();
    }

    public void UpdateMaxHP(float newMaxHP)
    {
        _hpSlider.maxValue = newMaxHP;
        _hpLayoutElement.flexibleWidth = newMaxHP;
        UpdateHPText();
    }

    private void UpdateHPText()
    {
        _hpText.text = $"{(int)_hpSlider.value} / {(int)_hpSlider.maxValue}";
    }
}
