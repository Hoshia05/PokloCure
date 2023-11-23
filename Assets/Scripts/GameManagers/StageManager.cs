using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageManager : MonoBehaviour
{
    public static StageManager instance;

    [Header("Stage Specific")]
    [SerializeField]
    private BoxCollider2D _spawnArea;
    [SerializeField]
    private CinemachineVirtualCamera _virtualCamera;
    [SerializeField]
    private Transform _characterSpawnPoint;

    [Header("Within Prefab")]
    [SerializeField]
    private Image _characterThumbnail;
    [SerializeField]
    private GameObject _levelUPUI;
    private LvlUPListScript _lvlUpListScript;
    [SerializeField]
    private GameObject _BoxItemUI;
    private BoxItemUIScript _boxItemUIScript;
    [SerializeField]
    private TextMeshProUGUI _timer;
    [SerializeField]
    private TextMeshProUGUI _killCountText;
    [SerializeField]
    private ItemSlotScript _itemSlotScript;

    private PlayerScript _currentPlayer;

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

        _lvlUpListScript = _levelUPUI.GetComponent<LvlUPListScript>();
        _boxItemUIScript = _BoxItemUI.GetComponent<BoxItemUIScript>();

        _killCountText.text = _killCount.ToString();
        _levelUPUI.SetActive(false);
        _BoxItemUI.SetActive(false);
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
            yield return new WaitForSeconds(0.5f);
            RandomEnemySpawn();
        }

    }

    private void RandomEnemySpawn()
    {
        //TODO:
        //적이 캐릭터 중심으로 원형으로 소환

        Vector2 randomPoint = new Vector2(UnityEngine.Random.Range(min.x, max.x), UnityEngine.Random.Range(min.y, max.y));

        System.Random random = new();

        EnemyBase randomEnemy = _enemyList[random.Next(0, _enemyList.Count)];

        GameObject newEnemy = Instantiate(GameManager.Instance.EnemyPrefab, randomPoint, Quaternion.identity);
        EnemyScript enemyScript = newEnemy.GetComponent<EnemyScript>();
        enemyScript.InitializeWithSO(randomEnemy);
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
        _levelUPUI.SetActive(true);
        _lvlUpListScript.CreateLvlUpList();
    }

    public void LevelUpEventEnd()
    {
        Time.timeScale = 1;
        _levelUPUI.SetActive(false);
    }

    public void FieldBoxEvent()
    {
        Time.timeScale = 0;
        _BoxItemUI.SetActive(true);
        _boxItemUIScript.InitializeUI();
    }

    public void EndBoxUI()
    {
        Time.timeScale = 1;
        _BoxItemUI.SetActive(false);
    }

    public void GivePlayerItem(ItemSO item)
    {
        _currentPlayer.ObtainItemSkill(item);
        _itemSlotScript.UpdateItemSlot(_currentPlayer);
    }
}
