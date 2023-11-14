using System.Collections;
using System.Collections.Generic;
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
        List<ItemController> PlayerWeapons = currentPlayer.Weapons;
        List<ItemController> PlayerItems = currentPlayer.Items;

        if(PlayerWeapons.Count > 0 )
        {
            for (int i = 0; i < PlayerWeapons.Count; i++)
            {
                if (PlayerWeapons[i] != null)
                {
                    Image ObjectImage = Weapons[i].GetComponent<Image>();
                    ObjectImage.sprite = PlayerWeapons[i].ItemData.ItemImage;
                }

            }
        }


        if (PlayerItems.Count > 0)
        {
            for (int i = 0; i < PlayerWeapons.Count; i++)
            {
                if (PlayerItems[i] != null)
                {
                    Image ObjectImage = Items[i].GetComponent<Image>();
                    ObjectImage.sprite = PlayerItems[i].ItemData.ItemImage;
                }
            }
        }


    }

}
