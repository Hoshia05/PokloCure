using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    public static HPBar Instance;

    private Slider _hpSlider;
    [SerializeField]
    private TextMeshProUGUI _hpText;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;

        _hpSlider = GetComponent<Slider>();

        _hpSlider.minValue = 0;
    }

    public void InitializeHPBar(float initialHP)
    {
        _hpSlider.maxValue = initialHP;
        _hpSlider.value = initialHP;
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
        UpdateHPText();
    }

    private void UpdateHPText()
    {
        _hpText.text = $"{(int)_hpSlider.value} / {(int)_hpSlider.maxValue}";
    }
}
