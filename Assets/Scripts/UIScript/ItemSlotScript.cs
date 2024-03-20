using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotScript : MonoBehaviour
{
    public static ItemSlotScript Instance;


    [SerializeField]
    private GameObject _weaponPrefab;
    [SerializeField]
    private GameObject _utilityPrefab;

    [SerializeField]
    private GameObject _weaponParent;
    [SerializeField]
    private GameObject _utilityParent;


    private List<GameObject> Weapons = new();
    private List<GameObject> Items = new();

    //[SerializeField]
    //private GameObject[] Weapons = new GameObject[3];
    //[SerializeField]
    //private GameObject[] Items = new GameObject[3];

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    public void SlotUpdate(PlayerScript currentPlayer)
    {
        for(int i = Weapons.Count; i < currentPlayer.WeaponSlotCount; i++)
        {
            GameObject weaponSlot = Instantiate(_weaponPrefab, _weaponParent.transform);
            Weapons.Add(weaponSlot);
        }

        for (int i = Items.Count; i < currentPlayer.ItemSlotCount; i++)
        {
            GameObject weaponSlot = Instantiate(_utilityPrefab, _utilityParent.transform);
            Items.Add(weaponSlot);
        }
    }


    public void UpdateItemSlot(PlayerScript currentPlayer)
    {
        //List<ItemController> PlayerWeapons = currentPlayer.Weapons;
        Dictionary<ItemSO,ItemController> PlayerWeapons = currentPlayer.Weapons;
        Dictionary<ItemSO, ItemController> PlayerItems = currentPlayer.Items;

        if(PlayerWeapons.Count > 0 )
        {
            for (int i = 0; i < PlayerWeapons.Count; i++)
            {
                if (PlayerWeapons.ElementAt(i).Key != null)
                {
                    Image ObjectImage = Weapons[i].GetComponent<Image>();
                    ObjectImage.sprite = PlayerWeapons.ElementAt(i).Value.ItemData.ItemImage;

                    ObjectImage.color += new Color(0f, 0f, 0f, 1f);

                    TextMeshProUGUI lvlText = Weapons[i].GetComponentInChildren<TextMeshProUGUI>();

                    string currentWeaponLevel = PlayerWeapons.ElementAt(i).Value.CurrentWeaponLevel.ToString();
                    lvlText.text = $"Lv.{currentWeaponLevel}";
                }

            }
        }


        if (PlayerItems.Count > 0) 
        {
            for (int i = 0; i < PlayerItems.Count; i++)
            {
                if (PlayerItems.ElementAt(i).Key != null)
                {
                    Image ObjectImage = Items[i].GetComponent<Image>();
                    ObjectImage.sprite = PlayerItems.ElementAt(i).Value.ItemData.ItemImage;

                    ObjectImage.color += new Color(0f, 0f, 0f, 1f);

                    TextMeshProUGUI lvlText = Items[i].GetComponentInChildren<TextMeshProUGUI>();

                    string currentItemLevel = PlayerItems.ElementAt(i).Value.CurrentWeaponLevel.ToString();
                    lvlText.text = $"Lv.{currentItemLevel}";
                }
            }
        }


    }

}
