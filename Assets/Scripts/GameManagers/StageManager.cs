using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageManager : MonoBehaviour
{
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

    private PlayerScript _currentPlayer;

    private Vector2 min;
    private Vector2 max;


    [SerializeField]
    private List<GameObject> _enemyList;

    private void Awake()
    {
        min = _spawnArea.bounds.min;
        max = _spawnArea.bounds.max;
        _levelUPUI.SetActive(false);
    }

    private void Start()
    {
        _enemyList = GameManager.Instance.EnemyList;

        SpawnPlayerCharacter();
        StartCoroutine(EnemySpawnCoroutine());
    }

    private void SpawnPlayerCharacter()
    {
        GameObject playerCharacter = Instantiate(GameManager.Instance.PlayerCharacterPrefab, _characterSpawnPoint.position, Quaternion.identity);
        _currentPlayer = playerCharacter.GetComponent<PlayerScript>();
        _currentPlayer.InitializeWithSO(GameManager.Instance.SelectedCharacter);

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
        Vector2 randomPoint = new Vector2(Random.Range(min.x, max.x), Random.Range(min.y, max.y));

        System.Random random = new();

        GameObject randomEnemy = _enemyList[random.Next(0, _enemyList.Count)];

        Instantiate(randomEnemy, randomPoint, Quaternion.identity);
    }
}
