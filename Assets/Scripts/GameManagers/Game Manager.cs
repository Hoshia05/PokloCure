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

    public ItemSO GetRandomItem(List<ItemSO> exemptList = null, bool noUpgrades = false)
    {
        return _isDebug ? GetFromList(DebugItemList, exemptList, noUpgrades) : GetFromList(ItemList, exemptList, noUpgrades);
    }

    private ItemSO GetFromList(List<ItemSO> targetList, List<ItemSO> exemptList = null, bool noUpgrades = false)
    {
        IEnumerable<ItemSO> candidates = exemptList == null ? targetList : targetList.Except(exemptList);

        if (noUpgrades)
        {
            candidates = candidates.Where(x => x.ItemType != ItemType.UPGRADE);
        }

        // 캐릭터 전용 아이템 2배 가중치를 포함한 가중치 목록
        List<(ItemSO item, int weight)> weightedList = new();
        int totalWeight = 0;

        foreach (ItemSO item in candidates)
        {
            int weight = item.Priority;
            if (_selectedCharacter != null && item.characterLabel == _selectedCharacter.characterLabel)
            {
                weight *= 2;
            }

            if (weight <= 0)
                continue;

            weightedList.Add((item, weight));
            totalWeight += weight;
        }

        if (totalWeight <= 0)
            return null;

        int roll = Rand.Next(0, totalWeight);

        foreach ((ItemSO item, int weight) in weightedList)
        {
            if (roll < weight)
                return item;

            roll -= weight;
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
