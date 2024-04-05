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

    [Header("Prefabs")]
    public GameObject EnemyPrefab;
    public GameObject BossPrefab;

    public List<EnemyBase> EnemyList;

    public List<ItemSO> ItemList;
    public List<ItemSO> DebugItemList;
    public GameObject PlayerCharacterPrefab;

    public GameObject DamagePopUpPrefab;
    public GameObject CriticalDamagePopUpPrefab;

    public GameObject SteakPrefab;
    public GameObject DiamondPrefab;
    public GameObject TreasureBoxPrefab;
    public GameObject ExpItemPrefab;
    public GameObject HitEffectPrefab;

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

    public List<ItemSO> GetFullList()
    {
        ItemList.Shuffle();

        return _isDebug ? DebugItemList : ItemList;
    }

    public ItemSO GetRandomItem(List<ItemSO> exemptList = null)
    {
        return _isDebug ? GetFromList(DebugItemList, exemptList) : GetFromList(ItemList, exemptList);
    }

    private ItemSO GetFromList(List<ItemSO> targetList, List<ItemSO> exemptList = null)
    {
        if (!targetList.Except(exemptList).Any())
            return null;

        List<ItemSO> possibleItemList = targetList.Except(exemptList).ToList();

        int TotalWeight = possibleItemList.Sum(x => x.Priority);

        int priorityCheck = GameManager.Instance.Rand.Next(1, TotalWeight);

        foreach(ItemSO item in possibleItemList)
        {
            int currentPriority = item.Priority;
            if(item.characterLabel == _selectedCharacter.characterLabel)
            {
                currentPriority *= 2;
            }

            priorityCheck -= currentPriority;

            if (priorityCheck <= 0)
                return item;
        }

        return null;
    }

    //ForSelectScreen

    public void SelectBlueTiger()
    {
        _selectedCharacter = _blueTiger;
        MoveToPlayScene();
    }

    public void SelectGreenTiger()
    {
        _selectedCharacter = _greenTiger;
        MoveToPlayScene();
    }

    //SceneChange

    //public void MoveToTitleScene()
    //{
    //    SceneManager.LoadScene("TitleScreen");
    //}

    public void MoveToCharacterSelectionScreen()
    {
        SceneManager.LoadScene("SelectScreen");
    }

    public void MoveToPlayScene()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void MoveToNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void GameOverEvent()
    {

    }

    //Utilities

    public bool RollRandom(float chanceValue)
    {
        float randValue = (float)Rand.NextDouble() * 100;

        return randValue <= chanceValue;
    }
}
