using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInfoUIScript : MonoBehaviour
{
    public static CharacterInfoUIScript Instance;

    [SerializeField] private Image _characterImage;
    [SerializeField] private TextMeshProUGUI _characterNameSlot;
    [SerializeField] private TextMeshProUGUI _hpSlot;
    [SerializeField] private TextMeshProUGUI _atkSlot;
    [SerializeField] private TextMeshProUGUI _spdSlot;
    [SerializeField] private TextMeshProUGUI _crtSlot;
    [SerializeField] private TextMeshProUGUI _hasteSlot;

    private void Awake()
    {
        Instance = this;
    }

    public void SetCharacterPortrait(Sprite charactersprite)
    {
        _characterImage.sprite = charactersprite;
    }

    public void SetCharacterName(string characterName)
    {
        _characterNameSlot.text = characterName;
    }

    public void SetHP(float hp, float maxHP) 
    {
        _hpSlot.text = $"{hp}/{maxHP}";
    }

    public void SetAtk(float atkValue)
    {
        float percentage = (atkValue - 1) * 100;
        string formattedPercentage = (Mathf.Round(percentage * 100) / 100f).ToString();

        _atkSlot.text = $"+{formattedPercentage}%";
    }

    public void SetSpd(float spdValue)
    {
        float percentage = (spdValue - 1) * 100;
        string formattedPercentage = (Mathf.Round(percentage * 100) / 100f).ToString();


        _spdSlot.text = $"+{formattedPercentage}%";
    }

    public void SetCrt(float crtValue)
    {

        float percentage = crtValue * 100;
        string formattedPercentage = (Mathf.Round(percentage * 100) / 100f).ToString();

        _crtSlot.text = $"+{formattedPercentage}%";
    }

    public void SetHaste(float hasteValue)
    {
        float percentage = (hasteValue - 1) * 100;
        string formattedPercentage = (Mathf.Round(percentage * 100) / 100f).ToString();

        _hasteSlot.text = $"+{formattedPercentage}%";
    }
}
