﻿using Cinemachine;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditorInternal.Profiling.Memory.Experimental.FileFormat;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance;

    [Header("Stage Specific")]
    [SerializeField]
    private Collider2D _spawnArea;
    [SerializeField]
    private CinemachineVirtualCamera _virtualCamera;
    [SerializeField]
    private Transform _characterSpawnPoint;


    [Header("Prefab")]
    [SerializeField]
    private GameObject _alertPromptPrefab;
    [SerializeField]
    private GameObject _gameOverPromptPrefab;

    [Header("Within Prefab")]
    [SerializeField]
    private GameObject _darkScreen;
    [SerializeField]
    private GameObject _pauseScreen;
    [SerializeField]
    private Image _characterThumbnail;
    [SerializeField]
    private GameObject _levelUPUI;
    private LvlUPListScript _lvlUpListScript;
    [SerializeField]
    private GameObject _BoxItemUI;
    private BoxItemUIScript _boxItemUIScript;
    [SerializeField]
    private GameObject _CharacterInfoUI;
    [SerializeField]
    private TextMeshProUGUI _timer;
    [SerializeField]
    private TextMeshProUGUI _scoreCountText;
    [SerializeField]
    private TextMeshProUGUI _killCountText;
    [SerializeField]
    private TextMeshProUGUI _coinCountText;
    [SerializeField]
    private ItemSlotScript _itemSlotScript;

    [SerializeField]
    private GameObject _comboObject;
    [SerializeField]
    private TextMeshProUGUI _comboCountText;

    private EnemySpawnManager _spawnManager;

    private PlayerScript _currentPlayer;

    public PlayerScript CurrentPlayer { get { return _currentPlayer; } }

    private Vector2 min;
    private Vector2 max;

    private const int ENEMYLIMIT = 1000;

    private float _currentTime = 0f;
    private int _killCount = 0;

    [SerializeField]
    private int _coinCount = 0;
    public int CoinCount { get { return _coinCount; } }

    private int _currentCombo = 0;
    public int CurrentCombo { get { return _currentCombo; } }

    private int _score = 0;
    public int Score { get { return _score; } } 

    private int _currentEnemyCount = 0;


    public List<ItemSO> ItemExemptList;


    //[Header("Enemy Related")]

    public UnityEvent onEnemyKilled;

    public int CurrentEnemyCount 
    { 
      get { return _currentEnemyCount; } 
      set { _currentEnemyCount = value; }
    }

    private List<EnemyBase> _enemyList;
    private Dictionary<EnemyBase, List<GameObject>> _enemyPool = new();
    private List<GameObject> _expItemPool = new();

    private int _swarmCoefficient = 7;
    private int _mediumCoefficient = 3;
    private int _eliteCoefficeint = 1;

    private int _circleEnemyNum = 10;

    private int _totalEnemyCoefficient => _swarmCoefficient + _mediumCoefficient + _eliteCoefficeint;

    public delegate string EnhanceFunction();

    private List<EnhanceFunction> _enemyEnhanceFuncitons = new();

    [SerializeField]
    public float BuffInterval = 10f;
    [SerializeField]
    private float _circleSpawnInterval = 10f;
    [SerializeField]
    private float _mteSpawnInvterval = 15f;

    private bool _gamePaused;

    private void Awake()
    {
        Instance = this;

        min = _spawnArea.bounds.min;
        max = _spawnArea.bounds.max;

        _spawnManager = GetComponent<EnemySpawnManager>();

        _spawnManager.InitializeMinMax(min, max);

        _lvlUpListScript = _levelUPUI.GetComponent<LvlUPListScript>();
        _boxItemUIScript = _BoxItemUI.GetComponent<BoxItemUIScript>();

        _darkScreen.SetActive(true);
        _pauseScreen.SetActive(true);
        _levelUPUI.SetActive(true);
        _BoxItemUI.SetActive(true);
        _CharacterInfoUI.SetActive(true);

        _scoreCountText.text = Score.ToString();
        _killCountText.text = _killCount.ToString();
        _coinCountText.text = _coinCount.ToString();
        _darkScreen.SetActive(false);
        _pauseScreen.SetActive(false);
        _levelUPUI.SetActive(false);
        _BoxItemUI.SetActive(false);
        _CharacterInfoUI.SetActive(false);

        UpdateComboCounter();

        ItemExemptList = new();

    }

    private void Start()
    {
        _enemyList = GameManager.Instance.EnemyList;

        SpawnPlayerCharacter();
        EnemyObjectPoolCreate();
        ExpItemPoolCreate();
        InitializeEnhanceFunctions();

        StartCoroutine(EnemyCircleSpawnCoroutine());
        StartCoroutine(EnemyMTESpawnCoroutine());
        StartCoroutine(TimerUpdateCoroutine());

        SetupItemExemptList();
    }

    private void Update()
    {
        _currentTime += Time.deltaTime;
    }

    private void EnemyObjectPoolCreate()
    {
        foreach(EnemyBase enemy in _enemyList)
        {
            List<GameObject> enemyList = new();

            int Count = 1000;

            if(enemy.EnemyClass == EnemyClass.MEDIUM)
            {
                Count = 150;
            }
            else if(enemy.EnemyClass == EnemyClass.ELITE)
            {
                Count = 100;
            }

            for(int i = 0; i < Count; i++)
            {
                GameObject newEnemy = Instantiate(GameManager.Instance.EnemyPrefab);
                EnemyScript enemyScript = newEnemy.GetComponent<EnemyScript>();
                enemyScript.InitializeWithSO(enemy);
                newEnemy.SetActive(false);

                enemyList.Add(newEnemy);
            }

            _enemyPool.Add(enemy, enemyList);
        }
    }

    private void ExpItemPoolCreate()
    {
        int Count = 2000;
        for(int i = 0; i < Count; i++)
        {
            GameObject newEXPItem = Instantiate(GameManager.Instance.ExpItemPrefab);
            newEXPItem.SetActive(false);

            _expItemPool.Add(newEXPItem);
        }
    }

    public void GetEXPItemFromPool(Vector2 position, float expValue)
    {
        GameObject expItem = _expItemPool.First(x => x.activeSelf == false);

        if (expItem == null)
            return;

        expItem.GetComponent<ExpItemScript>().Activate(expValue);
        expItem.transform.position = position;
    }

    private void InitializeEnhanceFunctions()
    {
        _enemyEnhanceFuncitons.Add(IncreaseSwarm);
        _enemyEnhanceFuncitons.Add(IncreaseMedium);
        _enemyEnhanceFuncitons.Add(IncreaseElite);
        _enemyEnhanceFuncitons.Add(IncreaseAll);
        _enemyEnhanceFuncitons.Add(BuffSwarm);
        _enemyEnhanceFuncitons.Add(BuffMedium);
        _enemyEnhanceFuncitons.Add(BuffElite);
        _enemyEnhanceFuncitons.Add(ShortenInterval);
    }

    private void SetupItemExemptList()
    {
        List<ItemSO> itemList = GameManager.Instance.GetFullList();

        foreach (ItemSO item in itemList)
        {
            if(ItemExemptList.Contains(item))
            {
                continue;
            }

            if(item.ItemType == ItemType.UPGRADE)
            {

            }

            if(item.characterLabel != _currentPlayer.CharacterLabel && item.characterLabel != CharacterDistinct.NONE)
            {
                //when this item is not this character's dedicated item
                ItemExemptList.Add(item);
            }
            else if(_currentPlayer.CheckItemPossessionLevel(item) > item.ItemMaxLevel  )
            {
                //when this item is already full level
                ItemExemptList.Add(item);
            }
            else if(item.ItemType == ItemType.WEAPON && _currentPlayer.CheckItemPossession(item) == null && _currentPlayer.ObtainedWeaponCount == _currentPlayer.WeaponSlotCount)
            {
                //when weapon slot is full and this item is a weapon
                ItemExemptList.Add(item);
            }
            else if (item.ItemType == ItemType.ITEM && _currentPlayer.CheckItemPossession(item) == null && _currentPlayer.ObtainedItemCount == _currentPlayer.ItemSlotCount)
            {
                //when weapon slot is full and this item is a weapon
                ItemExemptList.Add(item);
            }
        }
    }

    private void SpawnPlayerCharacter()
    {
        GameObject playerCharacter = Instantiate(GameManager.Instance.PlayerCharacterPrefab, _characterSpawnPoint.position, Quaternion.identity);
        _currentPlayer = playerCharacter.GetComponent<PlayerScript>();

        _characterThumbnail.sprite = GameManager.Instance.SelectedCharacter.CharacterPortrait;
        _virtualCamera.Follow = playerCharacter.transform;

        PlayerControl.Instance.PauseMenu.AddListener(PauseMenu);
        _currentPlayer.onTakeDamage.AddListener(ComboBreak);

        //slot initialize
        ItemSlotScript.Instance.SlotUpdate(_currentPlayer);
    }

    IEnumerator EnemyCircleSpawnCoroutine()
    {
        while (true)
        {
            float max = _circleSpawnInterval + 2f;
            float min = _circleSpawnInterval - 2f;
            if (min < 1)
                min = 1f;

            float waitTime = UnityEngine.Random.Range(min, max);
            yield return new WaitForSeconds(waitTime);
            if (_currentEnemyCount < ENEMYLIMIT)
            {
                BasicCircleSpawn();
            }
        }
    }

    IEnumerator EnemyMTESpawnCoroutine()
    {
        while (true)
        {
            float max = _mteSpawnInvterval + 2f;
            float min = _mteSpawnInvterval - 2f;
            if (min < 1)
                min = 1f;

            float waitTime = UnityEngine.Random.Range(min, max);
            yield return new WaitForSeconds(waitTime);
            if (_currentEnemyCount < ENEMYLIMIT)
            {
                MTESpawn();
            }
        }
    }

    private void BasicCircleSpawn()
    {
        List<Vector2> spawnPositionList = new();

        PlayerScript playerScript = StageManager.Instance.CurrentPlayer;
        Vector2 playerPosition = playerScript.transform.position;

        float radius = 30f;

        float degreeDif = 360 / _circleEnemyNum;

        float currentDegree = GameManager.Instance.Rand.Next(0, 360);

        for (float i = 0; i <= _circleEnemyNum; i++)
        {
            float xPosition = playerPosition.x + radius * Mathf.Cos(currentDegree);
            float yPosition = playerPosition.y + radius * Mathf.Sin(currentDegree);

            Vector2 newSpawnPosition = new Vector2(xPosition, yPosition);

            if (_spawnManager.CheckIfIsInBounds(newSpawnPosition))
            {
                spawnPositionList.Add(newSpawnPosition);
            }

            currentDegree += degreeDif;
        }

        foreach(Vector2 spawnPosition in spawnPositionList)
        {
            PullFromObjectPool(EnemyClass.SWARM, spawnPosition);
        }

    }

    private void MTESpawn()
    {
        float outerRadius = 80f;
        float innerRadius = 60f;

        Vector2 SpawnPosition = EnemySpawnManager.GetRandomPositionInPlayerRadius(outerRadius, innerRadius);

        while (!_spawnManager.CheckIfIsInBounds(SpawnPosition))
        {
            SpawnPosition = EnemySpawnManager.GetRandomPositionInPlayerRadius(outerRadius, innerRadius);
        }

        Vector2 LastEnemyPosition = SpawnPosition;

        for (int i = 0; i < _totalEnemyCoefficient; i++)
        {
            LastEnemyPosition = EnemySpawnManager.GetRandomPositionInRadius(LastEnemyPosition, 5f);
            PullFromObjectPool(GetClassBasedOnCoefficient(), LastEnemyPosition);
        }

    }

    private EnemyClass GetClassBasedOnCoefficient()
    {
        double randDouble = GameManager.Instance.Rand.NextDouble() * 1000;

        float randNum = (float)(randDouble % _totalEnemyCoefficient);


        if(randNum < _swarmCoefficient)
        {
            return EnemyClass.SWARM;
        }
        else if( randNum < _swarmCoefficient + _mediumCoefficient)
        {
            return EnemyClass.MEDIUM;
        }
        else
        {
            return EnemyClass.ELITE;
        }

    }

    private void PullFromObjectPool(EnemyClass enemyClass, Vector2 spawnPosition)
    {
        EnemyBase enemyType;

        List<EnemyBase> enemyBaseList = _enemyPool.Keys.ToList().Where(x => x.EnemyClass == enemyClass).ToList();

        if(enemyBaseList.Count == 1)
        {
            enemyType = enemyBaseList[0];
        }
        else
        {
            enemyType = enemyBaseList[GameManager.Instance.Rand.Next(0, enemyBaseList.Count - 1)];
        }

        List<GameObject> enemyQueue = _enemyPool[enemyType];

        if (enemyQueue == null || enemyQueue.Count == 0)
            return;

        GameObject enemy = enemyQueue.First(x => x.activeSelf == false) ;

        if (enemy == null)
            return;

        SetEnemyActive(enemy);

        //EnemyScript enemyScript = enemy.GetComponent<EnemyScript>();
        //enemyScript.InitializeWithSO(enemyType);
        //enemy.SetActive(true);
        //_currentEnemyCount++;

        enemy.transform.position = spawnPosition;

    }

    private void SetEnemyActive(GameObject enemy)
    {
        EnemyScript enemyScript = enemy.GetComponent<EnemyScript>();
        enemy.SetActive(true);
        enemy.GetComponent<Collider2D>().enabled = true;
        enemyScript.SpriteRenderer.enabled = true;
        enemyScript.InitializeWithSO(null);
        _currentEnemyCount++;
    }

    public void EnemyDeathEvent(GameObject enemyObject)
    {
        onEnemyKilled.Invoke();
        _currentEnemyCount--;
        enemyObject.SetActive(false);
    }

    //적 강화 관련

    private string IncreaseSwarm()
    {
        _swarmCoefficient *= 3;

        return "하급 적이 더 많이 등장합니다!";
    }

    private string IncreaseMedium()
    {
        _mediumCoefficient *= 3;
        return "중급 적이 더 많이 등장합니다!";
    }

    private string IncreaseElite()
    {
        _eliteCoefficeint *= 2;
        return "상급 적이 더 많이 등장합니다!";
    }

    private string IncreaseAll()
    {
        _swarmCoefficient *= 2;
        _mediumCoefficient *= 2;
        _eliteCoefficeint *= 2;
        return "모든 적이 더 많이 등장합니다!";
    }

    private string BuffSwarm()
    {
        BuffEnemy(EnemyClass.SWARM);
        return "하급 적이 더 강해집니다!";
    }

    private string BuffMedium()
    {
        BuffEnemy(EnemyClass.MEDIUM);
        return "중급 적이 더 강해집니다!";
    }

    private string BuffElite()
    {
        BuffEnemy(EnemyClass.ELITE);
        return "상급 적이 더 강해집니다!";
    }

    private string ShortenInterval()
    {
        _mteSpawnInvterval--;
        _circleSpawnInterval--;

        if (_mteSpawnInvterval == 1)
        {
            _enemyEnhanceFuncitons.Remove(ShortenInterval);
        }

        return "적들이 더 자주 등장합니다!";
    }

    private void BuffEnemy(EnemyClass targetClass)
    {
        foreach(KeyValuePair<EnemyBase, List<GameObject>> enemyType in _enemyPool)
        {
            if(enemyType.Key.EnemyClass == targetClass)
            {
                foreach(GameObject enemy in enemyType.Value)
                {
                    EnemyScript enemyScript = enemy.GetComponent<EnemyScript>();
                    enemyScript.BuffEnemy();
                }
            }
        }
    }


    public void EnemyBuffEvent()
    {
        int randNum = (int)((GameManager.Instance.Rand.NextDouble() * 20) % (_enemyEnhanceFuncitons.Count - 1));

        string promptText = _enemyEnhanceFuncitons[randNum]();

        _circleEnemyNum += 2;

        GameObject AlertPrompt = Instantiate(_alertPromptPrefab);
        AlertPromptScript promptScript = AlertPrompt.GetComponent<AlertPromptScript>();
        promptScript.ChangePrompt(promptText);
        Destroy(AlertPrompt, 3f);

    }


    //적 강화 END

    int GetEnemyNum()
    {
        if(_currentTime > 1800f)
        {
            return 20;
        }

        float spawnProbability = Sigmoid(_currentTime / 1800f) - 0.5f;

        int RandomAdd = GameManager.Instance.Rand.Next(1, 5);

        return Mathf.CeilToInt(spawnProbability * 20) + RandomAdd;
    }

    public static float Sigmoid(float x)
    {
        return 1f / (1f + Mathf.Exp(-x));
    }

    IEnumerator TimerUpdateCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.3f);
            UpdateTimer();
        }
    }

    private void UpdateTimer()
    {
        int minutes = Mathf.FloorToInt(_currentTime / 60F);
        int seconds = Mathf.FloorToInt(_currentTime - minutes * 60);
        string timerString = string.Format("{00:00}:{01:00}", minutes, seconds);

        _timer.text = timerString;

        if(_currentTime % BuffInterval < 0.3)
            EnemyBuffEvent();
    }

    public void UpdateKill(int score)
    {
        _killCount++;
        _currentCombo++;


        _killCountText.text = _killCount.ToString();
        UpdateScore(score);
        UpdateComboCounter();
    }

    public void UpdateScore(int score)
    {
        if(_currentCombo == 0)
        {
            _score += score;
        }
        else
        {
            _score += (score * _currentCombo);
        }

        _scoreCountText.text = _score.ToString();
    }

    public void ComboBreak()
    {
        _currentCombo = 0;
        UpdateComboCounter();
    }

    public void UpdateComboCounter()
    {
        _comboCountText.text = _currentCombo.ToString();

        if( _currentCombo == 0)
        {
            _comboObject.SetActive(false);
        }
        else
        {
            _comboObject.SetActive(true);
        }
    }

    public void LevelUpEvent()
    {
        Time.timeScale = 0;
        SetupItemExemptList();
        OpenCharacterInfoUI();
        _darkScreen.SetActive(true);
        _levelUPUI.SetActive(true);
        _lvlUpListScript.CreateLvlUpList();
    }

    public void LevelUpEventEnd()
    {
        Time.timeScale = 1;
        CloseCharacterInfoUI();
        _levelUPUI.SetActive(false);
        _darkScreen.SetActive(false);
    }

    public void FieldBoxEvent()
    {
        Time.timeScale = 0;
        SetupItemExemptList();
        _darkScreen.SetActive(true);
        _BoxItemUI.SetActive(true);
        _boxItemUIScript.InitializeUI();
    }

    public void EndBoxUI()
    {
        Time.timeScale = 1;
        _BoxItemUI.SetActive(false);
        _darkScreen.SetActive(false);
    }

    public void GameOverEvent()
    {

        PlayerControl.Instance.PauseMenu.RemoveListener(PauseMenu);
        PlayerScript.Instance.onTakeDamage.RemoveListener(ComboBreak);

        Time.timeScale = 0;
        GameObject AlertPrompt = Instantiate(_gameOverPromptPrefab);
        GameOverPromptScript GOPScript = AlertPrompt.GetComponent<GameOverPromptScript>();
        GOPScript.SetScore(Score);
    }

    public void PauseMenu()
    {
        if (_gamePaused)
        {
            _gamePaused = false;
            Time.timeScale = 1;
            _currentPlayer.IsPaused = false;


            _darkScreen.SetActive(false);
            _pauseScreen.SetActive(false);
        }
        else
        {
            _gamePaused = true;
            Time.timeScale = 0;
            _currentPlayer.IsPaused = true;


            _darkScreen.SetActive(true);
            _pauseScreen.SetActive(true);
        }
    }

    public void OpenCharacterInfoUI()
    {
        _CharacterInfoUI.SetActive(true);
        PlayerScript.Instance.UpdateInfoUI();
    }

    public void CloseCharacterInfoUI()
    {
        _CharacterInfoUI.SetActive(false);
    }

    public void GivePlayerItem(ItemSO item)
    {
        _currentPlayer.ObtainItemSkill(item);
        _itemSlotScript.UpdateItemSlot(_currentPlayer);
    }

    public void GainCoins(int coinValue)
    {
        _coinCount += coinValue;
        _coinCountText.text = _coinCount.ToString();
    }
}
