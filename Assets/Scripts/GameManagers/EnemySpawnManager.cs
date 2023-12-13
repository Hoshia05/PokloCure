using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour 
{
    private Vector2 _min;
    private Vector2 _max;

    public void InitializeMinMax(Vector2 min, Vector2 max)
    {
        _min = min;
        _max = max;
    }

    public void CircleSpawn(EnemyBase enemyData, float radius, int enemyNum)
    {
        List<Vector2> spawnPositionList = new();

        PlayerScript playerScript = StageManager.instance.CurrentPlayer;
        Vector2 playerPosition = playerScript.transform.position;


        float degreeDif = 360 / enemyNum;

        float currentDegree = 0;

        for(float i = 0; i <= enemyNum; i ++)
        {
            float xPosition = playerPosition.x + radius * Mathf.Cos(currentDegree);
            float yPosition = playerPosition.y + radius * Mathf.Sin(currentDegree);

            Vector2 newSpawnPosition = new Vector2(xPosition, yPosition);

            if(CheckIfIsInBounds(newSpawnPosition))
            {
                spawnPositionList.Add(newSpawnPosition);
            }

            currentDegree += degreeDif;
        }

        foreach(Vector2 spawnPosition in spawnPositionList)
        {
            GameObject newEnemy = Instantiate(GameManager.Instance.EnemyPrefab, spawnPosition, Quaternion.identity);
            EnemyScript enemyScript = newEnemy.GetComponent<EnemyScript>();
            enemyScript.InitializeWithSO(enemyData);
            StageManager.instance.CurrentEnemyCount++;
        }

    }

    public void SpawnBossEnemy(EnemyBase enemyData, float radius)
    {
        Vector2 SpawnPosition;

        SpawnPosition = GetRandomPositionInRadius(radius);

        while (!CheckIfIsInBounds(SpawnPosition))
        {
            SpawnPosition = GetRandomPositionInRadius(radius);
        }

        GameObject BossEnemy = Instantiate(GameManager.Instance.BossPrefab, SpawnPosition, Quaternion.identity);
        EnemyScript enemyScript = BossEnemy.GetComponent<BossScript>();
        enemyScript.InitializeWithSO(enemyData);
        StageManager.instance.CurrentEnemyCount++;

    }

    public Vector2 GetRandomPositionInRadius(float radius)
    {
        PlayerScript playerScript = StageManager.instance.CurrentPlayer;
        Vector2 playerPosition = playerScript.transform.position;

        float currentDegree = 360 / (GameManager.Instance.Rand.Next(0, 360));

        float xPosition = playerPosition.x + radius * Mathf.Cos(currentDegree);
        float yPosition = playerPosition.y + radius * Mathf.Sin(currentDegree);

       return new Vector2(xPosition, yPosition);
    }

    public bool CheckIfIsInBounds(Vector2 position)
    {
        return position.x > _min.x && position.x < _max.x && position.y > _min.y && position.y < _max.y ? true : false;
    }

}
