using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIShieldBar : MonoBehaviour
{
    public static UIShieldBar Instance;

    private Slider _shieldSlider;
    [SerializeField]
    private TextMeshProUGUI _shieldText;

    private LayoutElement _shieldLayoutElement;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;

        _shieldSlider = GetComponent<Slider>();
        _shieldLayoutElement = GetComponent<LayoutElement>();

        gameObject.SetActive(false);
    }

    // Update is called once per frame
    public void UpdateShield(float HPValue)
    {
        _shieldSlider.value = HPValue;
        UpdateHPText();
    }

    public void NewShieldValue(float newMaxShield)
    {
        gameObject.SetActive(true);
        _shieldSlider.maxValue = newMaxShield;
        _shieldSlider.value = newMaxShield;
        _shieldLayoutElement.flexibleWidth = newMaxShield;
        UpdateHPText();
    }

    private void UpdateHPText()
    {
        _shieldText.text = $"{(int)_shieldSlider.value} / {(int)_shieldSlider.maxValue}";

        if(_shieldSlider.value == 0)
        {
            _shieldSlider.maxValue = 0;
            gameObject.SetActive(false);
        }
    }
}
