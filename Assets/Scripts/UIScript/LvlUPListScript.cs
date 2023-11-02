using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LvlUPListScript : MonoBehaviour
{
    private const int _itemCount = 4;

    [SerializeField]
    private GameObject _lvlUpChoicePrefab;
    [SerializeField]
    private GameObject _itemListBase;

    private GameObject[] _itemList;

    private void Awake()
    {
        _itemList = new GameObject[_itemCount];
    }


}
