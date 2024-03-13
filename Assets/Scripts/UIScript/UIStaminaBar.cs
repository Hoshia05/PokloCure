using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIStaminaBar : MonoBehaviour
{
    public static UIStaminaBar Instance { get; private set; }

    private Slider _staminaSlider;

    [SerializeField]
    private Image _baseBar;

    private void Awake()
    {
        Instance = this;

        _staminaSlider = GetComponent<Slider>();

        _staminaSlider.minValue = 0;
    }

    public void InitializeStaminaBar(float initialStamina)
    {
        _staminaSlider.maxValue = initialStamina;
        _staminaSlider.value = initialStamina;
    }

    public void UpdateStamina(float staminaValue)
    {
        _staminaSlider.value = staminaValue;
    }

    public void StartRedline()
    {
        _baseBar.color = Color.red;
    }

    public void EndRedline()
    {
        _baseBar.color = Color.white;
    }
}
