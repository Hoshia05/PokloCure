using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotScript : MonoBehaviour
{
    public static ItemSlotScript Instance;

    [SerializeField]
    private GameObject[] Weapons = new GameObject[7];
    [SerializeField]
    private GameObject[] Items = new GameObject[7];

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    
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

                    TextMeshProUGUI lvlText = Weapons[i].GetComponentInChildren<TextMeshProUGUI>();
                    //lvlText.text = 
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
                }
            }
        }


    }

}
