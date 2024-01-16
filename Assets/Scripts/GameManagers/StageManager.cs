using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StageManager : MonoBehaviour
{
    public static StageManager instance;

    [Header("Stage Specific")]
    [SerializeField]
    private Collider2D _spawnArea;
    [SerializeField]
    private CinemachineVirtualCamera _virtualCamera;
    [SerializeField]
    private Transform _characterSpawnPoint;
    [SerializeField]
    private EnemySpawnProfileSO _spawnProfile;
    private List<SpawnInfo> _bossSpawnInfo;
    [SerializeField]
    private EnemyPatternSpawnProfileSO _patternSpawnProfile;
    private List<PatternSpawnInfo> _patternSpawnInfos;

    [Header("Within Prefab")]
    [SerializeField]
    private GameObject _darkScreen;
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
    private TextMeshProUGUI _killCountText;
    [SerializeField]
    private TextMeshProUGUI _coinCountText;
    [SerializeField]
    private ItemSlotScript _itemSlotScript;

    private EnemySpawnManager _spawnManager;

    private PlayerScript _currentPlayer;

    public PlayerScript CurrentPlayer { get { return _currentPlayer; } }

    private Vector2 min;
    private Vector2 max;

    private float _currentTime = 0f;
    private int _killCount = 0;
    private int _coinCount = 0;

    private int _currentEnemyCount = 0;

    private bool _summoningBoss = false;
    private bool _summoningPattern = false;

    public List<ItemSO> ItemExemptList;

    public int CurrentEnemyCount 
    { 
      get { return _currentEnemyCount; } 
      set { _currentEnemyCount = value; }
    }

    [SerializeField]
    private List<EnemyBase> _enemyList;

    private void Awake()
    {
        instance = this;

        min = _spawnArea.bounds.min;
        max = _spawnArea.bounds.max;

        _spawnManager = GetComponent<EnemySpawnManager>();

        _spawnManager.InitializeMinMax(min, max);

        _lvlUpListScript = _levelUPUI.GetComponent<LvlUPListScript>();
        _boxItemUIScript = _BoxItemUI.GetComponent<BoxItemUIScript>();

        _darkScreen.SetActive(true);
        _levelUPUI.SetActive(true);
        _BoxItemUI.SetActive(true);
        _CharacterInfoUI.SetActive(true);

        _killCountText.text = _killCount.ToString();
        _coinCountText.text = _coinCount.ToString();
        _darkScreen.SetActive(false);
        _levelUPUI.SetActive(false);
        _BoxItemUI.SetActive(false);
        _CharacterInfoUI.SetActive(false);

        _bossSpawnInfo = _spawnProfile.enemies.Where(x => x.EnemyData.isBossEnemy).ToList();
        _patternSpawnInfos = _patternSpawnProfile.patterns;

        ItemExemptList = new();
    }

    private void Start()
    {
        _enemyList = GameManager.Instance.EnemyList;

        SpawnPlayerCharacter();
        StartCoroutine(EnemySpawnCoroutine());
        StartCoroutine(TimerUpdateCoroutine());

        SetupItemExemptList();
    }

    private void Update()
    {
        _currentTime += Time.deltaTime;

        BossSpawn();
        PatternSpawn();

    }

    private void SetupItemExemptList()
    {
        List<ItemSO> itemList = GameManager.Instance.ItemList;

        foreach (ItemSO item in itemList)
        {
            if(item.characterLabel != _currentPlayer.CharacterLabel && item.characterLabel != CharacterDistinct.NONE)
                ItemExemptList.Add(item);
        }

    }

    private void SpawnPlayerCharacter()
    {
        GameObject playerCharacter = Instantiate(GameManager.Instance.PlayerCharacterPrefab, _characterSpawnPoint.position, Quaternion.identity);
        _currentPlayer = playerCharacter.GetComponent<PlayerScript>();

        _characterThumbnail.sprite = GameManager.Instance.SelectedCharacter.CharacterPortrait;
        _virtualCamera.Follow = playerCharacter.transform;
    }

    IEnumerator EnemySpawnCoroutine()
    {
        while (true)
        {
            float waitTime = GameManager.Instance.Rand.Next(1, 7);
            yield return new WaitForSeconds(waitTime);
            if (_currentEnemyCount < 200)
            {
                RandomCircleSpawn();
            }

        }
    }

    private void BossSpawn()
    {
        if (_bossSpawnInfo.Count == 0 || _summoningBoss == true)
            return;

        SpawnInfo NextBoss = _bossSpawnInfo.First();

        float Spawntime = NextBoss.StartTime; 

        if(Math.Abs(Spawntime - _currentTime) < 0.3f)
        {
            _summoningBoss = true;

            _spawnManager.SpawnBossEnemy(NextBoss.EnemyData, 30);
            _bossSpawnInfo.Remove(NextBoss);

            _summoningBoss = false;
        }

    }

    private void PatternSpawn()
    {
        if(_patternSpawnInfos.Count == 0) 
            return;

        PatternSpawnInfo nextPattern = _patternSpawnInfos.First();

        if (Math.Abs(nextPattern.SpawnTime - _currentTime) < 0.3f)
        {
            _summoningPattern = true;

            _spawnManager.PatternSpawn(nextPattern, 30);
            _patternSpawnInfos.Remove(nextPattern);

            _summoningPattern = false;
        }
    }

    private List<EnemyBase> GetSpawnableEnemies()
    {
        List<EnemyBase> spawnableEnemies = _spawnProfile.enemies.Where(x => x.StartTime < _currentTime &&  _currentTime < x.EndTime).Select(x => x.EnemyData).ToList();

        return spawnableEnemies;
    }

    private void RandomEnemySpawn()
    {
        Vector2 randomPoint = new Vector2(UnityEngine.Random.Range(min.x, max.x), UnityEngine.Random.Range(min.y, max.y));

        System.Random random = new();

        EnemyBase randomEnemy = _enemyList[random.Next(0, _enemyList.Count)];

        GameObject newEnemy = Instantiate(GameManager.Instance.EnemyPrefab, randomPoint, Quaternion.identity);
        EnemyScript enemyScript = newEnemy.GetComponent<EnemyScript>();
        enemyScript.InitializeWithSO(randomEnemy);
    }

    private void RandomCircleSpawn()
    {
        Vector2 randomPoint = new Vector2(UnityEngine.Random.Range(min.x, max.x), UnityEngine.Random.Range(min.y, max.y));

        List<EnemyBase> EnemyPossibleList = GetSpawnableEnemies();

        if (EnemyPossibleList.Count == 0)
            return;

        foreach (EnemyBase enemyData in EnemyPossibleList)
        {
            int enemyNum = GetEnemyNum();

            _spawnManager.CircleSpawn(enemyData, 30, enemyNum);
        }
    }

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

    float Sigmoid(float x)
    {
        return 1f / (1f + Mathf.Exp(-x));
    }

    int WeightedRandomize(int max, float weight)
    {
        // Use weight to bias randomization
        float randomValue = UnityEngine.Random.Range(0f, 1f);
        float weightedRandomValue = Mathf.Pow(randomValue, weight);

        // Map weighted random value to the desired range
        return Mathf.RoundToInt(weightedRandomValue * max) + 1;
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
        string timerString = string.Format("{0:0}:{1:00}", minutes, seconds);

        _timer.text = timerString;
    }

    public void UpdateKill()
    {
        _killCount++;
        _killCountText.text = _killCount.ToString();
    }

    public void LevelUpEvent()
    {
        Time.timeScale = 0;
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
