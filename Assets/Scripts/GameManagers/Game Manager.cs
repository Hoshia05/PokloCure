using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public System.Random Rand = new();
    public GameObject EnemyPrefab;
    public GameObject BossPrefab;
    public List<EnemyBase> EnemyList;

    public List<ItemSO> ItemList;
    public List<ItemSO> DebugItemList;
    public GameObject PlayerCharacterPrefab;

    public GameObject DamagePopUpPrefab;
    public GameObject CriticalDamagePopUpPrefab;

    public GameObject BurgerPrefab;
    public GameObject DiamondPrefab;
    public GameObject TreasureBoxPrefab;
    public GameObject ExpItemPrefab;

    [Header("캐릭터데이터")]
    [SerializeField]
    private CharacterBase _greenTiger;
    [SerializeField]
    private CharacterBase _blueTiger;

    [Header("디버그용")]
    [SerializeField]
    private bool _isDebug;

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
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        DontDestroyOnLoad(gameObject);
    }

    public ItemSO GetRandomItem(List<ItemSO> exemptList = null)
    {
        return _isDebug ? GetFromDebugList(exemptList) : GetFromFullList(exemptList);
    }

    private ItemSO GetFromFullList(List<ItemSO> exemptList = null)
    {
        if (!ItemList.Except(exemptList).Any())
            return null;

        List<ItemSO> possibleItemList = ItemList.Except(exemptList).ToList();

        return possibleItemList[Rand.Next(possibleItemList.Count)];
    }

    private ItemSO GetFromDebugList(List<ItemSO> exemptList = null)
    {
        if (!DebugItemList.Except(exemptList).Any())
            return null;

        List<ItemSO> possibleItemList = DebugItemList.Except(exemptList).ToList();

        return possibleItemList[Rand.Next(possibleItemList.Count)];
    }

    //ForSelectScreen

    public void SelectBlueTiger()
    {
        _selectedCharacter = _blueTiger;
        MoveToNextScene();
    }

    public void SelectGreenTiger()
    {
        _selectedCharacter = _greenTiger;
        MoveToNextScene();
    }

    public void MoveToNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
