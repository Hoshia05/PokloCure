using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public System.Random Rand = new();
    public GameObject EnemyPrefab;
    public GameObject BossPrefab;
    public List<EnemyBase> EnemyList;
    public List<ItemSO> ItemList;
    public GameObject PlayerCharacterPrefab;

    public GameObject DamagePopUpPrefab;
    public GameObject CriticalDamagePopUpPrefab;

    public GameObject BurgerPrefab;
    public GameObject DiamondPrefab;
    public GameObject TreasureBoxPrefab;
    public GameObject ExpItemPrefab;

    [Header("디버그용")]
    [SerializeField]
    private CharacterBase _selectedCharacter;

    public CharacterBase SelectedCharacter
    {
        get 
        {
            return _selectedCharacter;
        }
        set
        {
            _selectedCharacter = value;
        }
    }

    private void Awake()
    {
        if(Instance == null)
            Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public ItemSO GetRandomItem(List<ItemSO> exemptList = null)
    {
        if (!ItemList.Except(exemptList).Any())
            return null;

        List<ItemSO> possibleItemList = ItemList.Except(exemptList).ToList();

        return possibleItemList[Rand.Next(possibleItemList.Count)];
    }

}
