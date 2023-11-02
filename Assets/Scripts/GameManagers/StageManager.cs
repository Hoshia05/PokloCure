using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    [SerializeField]
    private BoxCollider2D _spawnArea;

    private Vector2 min;
    private Vector2 max;


    [SerializeField]
    private List<GameObject> _enemyList = new List<GameObject>();

    private void Awake()
    {
        min = _spawnArea.bounds.min;
        max = _spawnArea.bounds.max;
    }

    private void Start()
    {
        StartCoroutine(EnemySpawnCoroutine());
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
