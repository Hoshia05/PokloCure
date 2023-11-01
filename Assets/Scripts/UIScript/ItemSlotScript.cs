using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    
    public void UpdateItemSlot()
    {
        //TODO;
    }

}
