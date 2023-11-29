using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
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
    private ItemSlotScript _itemSlotScript;

    private EnemySpawnManager _spawnManager;

    private PlayerScript _currentPlayer;

    public PlayerScript CurrentPlayer { get { return _currentPlayer; } }

    private Vector2 min;
    private Vector2 max;

    private float _currentTime = 0f;
    private int _killCount = 0;

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
        _darkScreen.SetActive(false);
        _levelUPUI.SetActive(false);
        _BoxItemUI.SetActive(false);
        _CharacterInfoUI.SetActive(false);
    }

    private void Start()
    {
        _enemyList = GameManager.Instance.EnemyList;

        SpawnPlayerCharacter();
        StartCoroutine(EnemySpawnCoroutine());
        StartCoroutine(TimerUpdateCoroutine());
    }

    private void Update()
    {
        _currentTime += Time.deltaTime;
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
            yield return new WaitForSeconds(5f);
            RandomCircleSpawn();
        }

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

        System.Random random = new();

        EnemyBase randomEnemy = _enemyList[random.Next(0, _enemyList.Count)];

        _spawnManager.CircleSpawn(randomEnemy, 30, 10);
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
}
