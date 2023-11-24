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
    [SerializeField] private TextMeshProUGUI _pickupSlot;
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
        _atkSlot.text = $"+{(atkValue-1) * 100}%";
    }

    public void SetSpd(float spdValue)
    {
        _spdSlot.text = $"+{(spdValue - 1) * 100}%";
    }

    public void SetCrt(float crtValue)
    {
        _crtSlot.text = $"+{crtValue * 100}%";
    }

    public void SetPickup(float pickupValue)
    {
        _pickupSlot.text = $"+{(pickupValue - 1) * 100}%";
    }

    public void SetHaste(float hasteValue)
    {
        _hasteSlot.text = $"+{(hasteValue - 1) * 100}%";
    }
}
