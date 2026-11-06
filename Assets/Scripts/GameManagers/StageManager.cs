using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
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

    [Header("Sounds")]
    [SerializeField]
    private AudioClip _pauseMenuSound;
    [SerializeField]
    private AudioClip _levelUpSound;

    [Header("Within Prefab")]
    [SerializeField]
    private GameObject _darkScreen;
    [SerializeField]
    private GameObject _pauseScreen;
    [SerializeField]
    private GameObject _soundScreen;
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

    private const int ENEMYLIMIT = 3000;

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


    public List<ItemSO> ItemExemptList = new();



    [HideInInspector]
    public UnityEvent onEnemyKilled;

    public int CurrentEnemyCount 
    { 
      get { return _currentEnemyCount; } 
      set { _currentEnemyCount = value; }
    }

    private List<EnemyBase> _enemyList;
    private Dictionary<EnemyBase, SimplePool> _enemyPool = new();
    private Dictionary<EnemyBase, List<EnemyScript>> _enemyScriptsByType = new();
    private Dictionary<EnemyClass, List<EnemyBase>> _enemyTypesByClass = new();
    private Dictionary<EnemyClass, float> _classBuffMultiplier = new();
    private SimplePool _expItemPool;
    private SimplePool _moneyItemPool;
    private SimplePool _damagePopupPool;
    private SimplePool _criticalDamagePopupPool;
    private SimplePool _enemyHitAnimationPool;

    [Header("Enemy Related")]
    [SerializeField]
    private int _swarmCoefficient = 15;
    [SerializeField]
    private int _mediumCoefficient = 3;
    [SerializeField]
    private int _eliteCoefficeint = 1;

    [SerializeField]
    private int _circleEnemyNum = 10;

    private int _totalEnemyCoefficient => _swarmCoefficient + _mediumCoefficient + _eliteCoefficeint;

    public delegate string EnhanceFunction();

    private List<EnhanceFunction> _enemyEnhanceFuncitons = new();

    [SerializeField]
    public float BuffInterval = 10f;
    [SerializeField]
    private float _circleSpawnInterval = 5f;
    [SerializeField]
    private float _mteSpawnInvterval = 15f;

    private bool _gamePaused;
    private bool _pauseEvent;

    private float _nextBuffTime;

    private void Awake()
    {
        Instance = this;

        Cursor.visible = false;

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

    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private void Start()
    {
        _enemyList = GameManager.Instance.EnemyList;

        SpawnPlayerCharacter();

        //GameObject topPoolParent = new GameObject("TopPoolParent");
        
        EnemyObjectPoolCreate();
        DamageUIPoolCreate();
        ExpItemPoolCreate();
        MoneyItemPoolCreate();
        EnemyHitAnimationpoolCreate();

        InitializeEnhanceFunctions();

        _nextBuffTime = BuffInterval;

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
        GameObject enemyObjectPoolParent = new GameObject("enemyObjectPoolParent");

        foreach(EnemyBase enemy in _enemyList)
        {
            EnemyBase enemyData = enemy;
            List<EnemyScript> scripts = new();

            // 처음 만들어 두는 개수 (풀은 필요하면 늘어난다. 전체 상한은 ENEMYLIMIT)
            int prewarm = 300;

            if(enemy.EnemyClass == EnemyClass.MEDIUM)
            {
                prewarm = 50;
            }
            else if(enemy.EnemyClass == EnemyClass.ELITE)
            {
                prewarm = 20;
            }

            _enemyScriptsByType.Add(enemyData, scripts);

            if (!_enemyTypesByClass.TryGetValue(enemyData.EnemyClass, out List<EnemyBase> typeList))
            {
                typeList = new();
                _enemyTypesByClass.Add(enemyData.EnemyClass, typeList);
            }
            typeList.Add(enemyData);

            SimplePool pool = new SimplePool(GameManager.Instance.EnemyPrefab, enemyObjectPoolParent.transform, prewarm, int.MaxValue, newEnemy =>
            {
                EnemyScript enemyScript = newEnemy.GetComponent<EnemyScript>();
                enemyScript.InitializeWithSO(enemyData);
                // 나중에 만들어진 적도 현재 강화 단계를 적용
                enemyScript.SetBuffMultiplier(GetClassBuffMultiplier(enemyData.EnemyClass));
                scripts.Add(enemyScript);
            });

            _enemyPool.Add(enemyData, pool);
        }
    }

    private float GetClassBuffMultiplier(EnemyClass enemyClass)
    {
        return _classBuffMultiplier.TryGetValue(enemyClass, out float multiplier) ? multiplier : 1f;
    }

    private void ExpItemPoolCreate()
    {

        GameObject expObjectPoolParent = new GameObject("expObjectPoolParent");

        // 필요할 때 늘어나는 풀: 처음 500개, 최대 4000개 (기존 상한 유지)
        _expItemPool = new SimplePool(GameManager.Instance.ExpItemPrefab, expObjectPoolParent.transform, 500, 4000);
    }

    public void GetEXPItemFromPool(Vector2 position, float expValue)
    {

        GameObject expItem = _expItemPool.Get();

        if (expItem == null)
            return;

        expItem.transform.position = position;
        expItem.GetComponent<ExpItemScript>().Activate(expValue);
    }

    private void MoneyItemPoolCreate()
    {

        GameObject moneyObjectPoolParent = new GameObject("moneyObjectPoolParent");

        // 처음 200개, 최대 2000개 (기존 상한 유지)
        _moneyItemPool = new SimplePool(GameManager.Instance.DiamondPrefab, moneyObjectPoolParent.transform, 200, 2000);
    }

    public void GetMoneyItemFromPool(Vector2 targetPosition)
    {
        GameObject moneyItem = _moneyItemPool.Get();

        if (moneyItem == null)
            return;

        moneyItem.SetActive(true);
        moneyItem.transform.position = targetPosition;

    }

    private void DamageUIPoolCreate()
    {

        GameObject damageUIObjectPoolParent = new GameObject("damageUIObjectPoolParent");

        // 처음 50개씩, 최대 150개씩
        _damagePopupPool = new SimplePool(GameManager.Instance.DamagePopUpPrefab, damageUIObjectPoolParent.transform, 50, 150);
        _criticalDamagePopupPool = new SimplePool(GameManager.Instance.CriticalDamagePopUpPrefab, damageUIObjectPoolParent.transform, 50, 150);
    }

    public void GetDamageUIFromPool(Vector2 position, float DamageValue, bool isCritical)
    {
        GameObject DamagePopup = isCritical ? _criticalDamagePopupPool.Get() : _damagePopupPool.Get();

        if (DamagePopup == null)
            return;

        DamagePopup.SetActive(true);
        DamagePopup.transform.position = position;

        TextMeshProUGUI tmp = DamagePopup.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        tmp.text = DamageValue.ToString();

        tmp.gameObject.transform.localPosition = Vector2.zero;

        Rigidbody2D tmpRB = tmp.GetComponent<Rigidbody2D>();

        double jumpRange = (GameManager.Instance.Rand.NextDouble() - 0.5) * 2;
        Vector2 direction = new Vector2((float)jumpRange, 1);

        float force = isCritical ? 200 : 150;

        tmpRB.AddForce(direction * force);

        StartCoroutine(DeactivateTimer(DamagePopup));

    }

    private void EnemyHitAnimationpoolCreate()
    {
        GameObject hitanimationObjectPoolParent = new GameObject("hitanimationObjectPoolParent");


        // 처음 50개, 최대 150개
        _enemyHitAnimationPool = new SimplePool(GameManager.Instance.HitEffectPrefab, hitanimationObjectPoolParent.transform, 50, 150);
    }

    public void GetHitAnimationFromPool(Vector2 targetPosition)
    {
        GameObject animationObject = _enemyHitAnimationPool.Get();

        if (animationObject == null)
            return;

        animationObject.transform.position = targetPosition;

        animationObject.SetActive(true);

    }

    IEnumerator DeactivateTimer(GameObject deactiveObject)
    {
        yield return new WaitForSeconds(0.5f);

        PooledObject.ReleaseOrDeactivate(deactiveObject);
    }

    private void InitializeEnhanceFunctions()
    {
        //_enemyEnhanceFuncitons.Add(IncreaseSwarm);
        //_enemyEnhanceFuncitons.Add(IncreaseMedium);
        //_enemyEnhanceFuncitons.Add(IncreaseElite);
        //_enemyEnhanceFuncitons.Add(IncreaseAll);
        _enemyEnhanceFuncitons.Add(BuffSwarm);
        _enemyEnhanceFuncitons.Add(BuffMedium);
        _enemyEnhanceFuncitons.Add(BuffElite);
        _enemyEnhanceFuncitons.Add(ShortenInterval);
    }

    private void SetupItemExemptList()
    {
        List<ItemSO> itemList = GameManager.Instance.GetFullList();

        ItemExemptList = new();

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

        _characterThumbnail.sprite = GameManager.Instance.SelectedCharacter.CharacterSprite;
        _virtualCamera.Follow = playerCharacter.transform;

        PlayerControl.Instance.PauseMenu.AddListener(PauseMenu);
        _currentPlayer.onTakeDamage.AddListener(ComboBreak);

        //slot initialize
        ItemSlotScript.Instance.SlotUpdate(_currentPlayer);
    }

    IEnumerator EnemyCircleSpawnCoroutine(int spawnNum = 1)
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
                for(int i = 0; i < spawnNum; i++)
                {
                    BasicCircleSpawn();
                    yield return new WaitForSeconds(1f);
                }

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

        for (float i = 0; i < _circleEnemyNum; i++)
        {
            float angle = i * 360f / _circleEnemyNum;
            float radians = Mathf.Deg2Rad * angle;

            float xPosition = playerPosition.x + (radius * Mathf.Cos(radians));
            float yPosition = playerPosition.y + (radius * Mathf.Sin(radians));

            Vector2 newSpawnPosition = new Vector2(xPosition, yPosition);

            if (_spawnManager.CheckIfIsInBounds(newSpawnPosition))
            {
                spawnPositionList.Add(newSpawnPosition);
            }


        }

        foreach (Vector2 spawnPosition in spawnPositionList)
        {
            PullEnemyFromObjectPool(RandomizeEnemyCircleSpawn(), spawnPosition);
        }

    }

    private EnemyClass RandomizeEnemyCircleSpawn()
    {
        // 현재 동작 유지: 원형 소환은 항상 하급(SWARM)만 소환한다.
        // (이전 코드는 Next(1, 10)이 1~9만 반환해서 사실상 항상 SWARM이었음. 밸런스 변경 시 여기서 조정할 것)
        return EnemyClass.SWARM;
    }

    private void MTESpawn()
    {
        float outerRadius = 80f;
        float innerRadius = 60f;

        if (!_spawnManager.TryGetPositionInPlayerRing(outerRadius, innerRadius, out Vector2 SpawnPosition))
            return;

        Vector2 LastEnemyPosition = SpawnPosition;

        for (int i = 0; i < _totalEnemyCoefficient; i++)
        {
            LastEnemyPosition = EnemySpawnManager.GetRandomPositionInRadius(LastEnemyPosition, 5f);
            PullEnemyFromObjectPool(GetClassBasedOnCoefficient(), LastEnemyPosition);
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

    private void PullEnemyFromObjectPool(EnemyClass enemyClass, Vector2 spawnPosition)
    {
        EnemyBase enemyType;

        if (_currentEnemyCount >= ENEMYLIMIT)
            return;

        if (!_enemyTypesByClass.TryGetValue(enemyClass, out List<EnemyBase> enemyBaseList) || enemyBaseList.Count == 0)
            return;

        if (enemyBaseList.Count == 1)
        {
            enemyType = enemyBaseList[0];
        }
        else
        {
            enemyType = enemyBaseList[GameManager.Instance.Rand.Next(0, enemyBaseList.Count)];
        }

        if (!_spawnManager.CheckIfIsInBounds(spawnPosition))
            return;

        GameObject enemy = _enemyPool[enemyType].Get();

        if (enemy == null)
            return;

        SetEnemyActive(enemy);

        enemy.transform.position = spawnPosition;

    }

    private void SetEnemyActive(GameObject enemy)
    {
        EnemyScript enemyScript = enemy.GetComponent<EnemyScript>();
        enemy.SetActive(true);
        enemyScript.ResetForSpawn();
        _currentEnemyCount++;
    }

    public void EnemyDeathEvent(GameObject enemyObject)
    {
        onEnemyKilled.Invoke();
        _currentEnemyCount--;
        PooledObject.ReleaseOrDeactivate(enemyObject);
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
        _swarmCoefficient += 5;
        _mediumCoefficient += 2;
        _eliteCoefficeint += 1;
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

        if (_mteSpawnInvterval <= 1f)
        {
            _mteSpawnInvterval = 1f;
            _circleSpawnInterval = Mathf.Max(1f, _circleSpawnInterval);
            _enemyEnhanceFuncitons.Remove(ShortenInterval);
        }

        return "적들이 더 자주 등장합니다!";
    }

    private void BuffEnemy(EnemyClass targetClass)
    {
        // 이후에 새로 만들어지는 적을 위해 클래스별 배율도 같이 올린다
        _classBuffMultiplier[targetClass] = GetClassBuffMultiplier(targetClass) + EnemyScript.BuffStep;

        foreach(KeyValuePair<EnemyBase, List<EnemyScript>> enemyType in _enemyScriptsByType)
        {
            if(enemyType.Key.EnemyClass == targetClass)
            {
                foreach(EnemyScript enemyScript in enemyType.Value)
                {
                    enemyScript.BuffEnemy();
                }
            }
        }
    }


    public void EnemyBuffEvent()
    {
        int randNum = GameManager.Instance.Rand.Next(_enemyEnhanceFuncitons.Count);

        string promptText = _enemyEnhanceFuncitons[randNum]();

        IncreaseAll();

        _circleEnemyNum += 2;

        GameObject AlertPrompt = Instantiate(_alertPromptPrefab);
        AlertPromptScript promptScript = AlertPrompt.GetComponent<AlertPromptScript>();
        promptScript.ChangePrompt(promptText);
        Destroy(AlertPrompt, 3f);

    }


    //적 강화 END

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

        if (_currentTime >= _nextBuffTime)
        {
            _nextBuffTime += BuffInterval;
            EnemyBuffEvent();
        }
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

    //Pause events
    void PauseStart()
    {
        Time.timeScale = 0;
        Cursor.visible = true;
        _gamePaused = true;
        _currentPlayer.IsPaused = true;


        _darkScreen.SetActive(true);
    }

    void PauseEnd()
    {
        Time.timeScale = 1;
        Cursor.visible = false;
        _gamePaused = false;
        _currentPlayer.IsPaused = false;


        _darkScreen.SetActive(false);
    }


    public void LevelUpEvent()
    {
        PauseStart();

        SoundFXManager.Instance.PlaySoundFXClip(_levelUpSound, transform, 1f);
        _pauseEvent = true;
        OpenCharacterInfoUI();
        _levelUPUI.SetActive(true);
        SetupItemExemptList();
        _lvlUpListScript.CreateLvlUpList();
    }

    public bool IsLevelUpOpen => _levelUPUI.activeSelf;

    public void LevelUpSkip()
    {
        PlayerScript.Instance.CheckLevelUp();
    }

    public void LevelUpEventEnd()
    {


        _pauseEvent = false;
        CloseCharacterInfoUI();
        _levelUPUI.SetActive(false);

        PauseEnd();
    }

    public void FieldBoxEvent()
    {
        PauseStart();

        _pauseEvent = true;
        SetupItemExemptList();
        _BoxItemUI.SetActive(true);
        _boxItemUIScript.InitializeUI();
    }

    public void EndBoxUI()
    {
        PauseEnd();

        _pauseEvent = false;
        _BoxItemUI.SetActive(false);
        _darkScreen.SetActive(false);
    }

    public void GameOverEvent()
    {

        PlayerControl.Instance.PauseMenu.RemoveListener(PauseMenu);
        PlayerScript.Instance.onTakeDamage.RemoveListener(ComboBreak);

        PauseStart();

        _pauseEvent = true;
        GameObject AlertPrompt = Instantiate(_gameOverPromptPrefab);
        GameOverPromptScript GOPScript = AlertPrompt.GetComponent<GameOverPromptScript>();
        GOPScript.SetScore(Score);
    }

    public void PauseMenu()
    {
        if (_pauseEvent)
            return;

        if (_gamePaused)
        {
            PauseEnd();

            SoundFXManager.Instance.PlaySoundFXClip(_pauseMenuSound, transform, 1f);

            CloseCharacterInfoUI();
            _pauseScreen.SetActive(false);
            _soundScreen.SetActive(false);
        }
        else
        {
            PauseStart();

            SoundFXManager.Instance.PlaySoundFXClip(_pauseMenuSound, transform, 1f);

            OpenCharacterInfoUI();
            _pauseScreen.SetActive(true);
        }
    }

    public void SoundMenu()
    {
        _soundScreen.SetActive(!_soundScreen.activeSelf);

        SoundMenuScript smScript = _soundScreen.GetComponent<SoundMenuScript>();
        smScript.SetSliders();
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
