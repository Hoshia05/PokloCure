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

        PlayerScript playerScript = StageManager.Instance.CurrentPlayer;
        Vector2 playerPosition = playerScript.transform.position;


        float degreeDif = 360 / enemyNum;

        float currentDegree = GameManager.Instance.Rand.Next(0, 360);

        for (float i = 0; i <= enemyNum; i ++)
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
            StageManager.Instance.CurrentEnemyCount++;
        }

    }

    public void SpawnBossEnemy(EnemyBase enemyData, float radius)
    {
        Vector2 SpawnPosition = GetRandomPositionInPlayerRadius(radius);

        while (!CheckIfIsInBounds(SpawnPosition))
        {
            SpawnPosition = GetRandomPositionInPlayerRadius(radius);
        }

        GameObject BossEnemy = Instantiate(GameManager.Instance.BossPrefab, SpawnPosition, Quaternion.identity);
        EnemyScript enemyScript = BossEnemy.GetComponent<BossScript>();
        enemyScript.InitializeWithSO(enemyData);
        StageManager.Instance.CurrentEnemyCount++;

    }

   

    //pattern spawning

    public void PatternSpawn(PatternSpawnInfo patternSpawnInfo, float radius)
    {
        if (patternSpawnInfo == null)
            return;

        switch (patternSpawnInfo.SpawnType)
        {
            case SpawnType.CLUSTER:
                ClusterSpawn(patternSpawnInfo, radius);
                break;
            case SpawnType.HORDE: 
                break;
            case SpawnType.WALL:
                break;
            case SpawnType.RING:
                break;
            case SpawnType.STAMPEDE:
                break;

        }
    }

    public void ClusterSpawn(PatternSpawnInfo patternSpawnInfo, float radius)
    {
        EnemyBase enemyData = patternSpawnInfo.EnemyData;
        int enemyNum = patternSpawnInfo.AmountSpawned;

        Vector2 SpawnPosition;

        SpawnPosition = GetRandomPositionInPlayerRadius(radius);

        while (!CheckIfIsInBounds(SpawnPosition))
        {
            SpawnPosition = GetRandomPositionInPlayerRadius(radius);
        }

        for (int i = 0; i < enemyNum; i++)
        {
            Vector2 IndividualSpawnPosition = GetRandomPositionInRadius(SpawnPosition, 2);

            GameObject ClusterEnemey = Instantiate(GameManager.Instance.EnemyPrefab, IndividualSpawnPosition, Quaternion.identity);
            EnemyScript enemyScript = ClusterEnemey.GetComponent<EnemyScript>();
            enemyScript.InitializeWithSO(enemyData);
        }
    }


    //utilities
    public static Vector2 GetRandomPositionInPlayerRadius(float outerRadius, float innerRadius = 0)
    {
        PlayerScript playerScript = StageManager.Instance.CurrentPlayer;
        Vector2 playerPosition = playerScript.transform.position;

        //return GetRandomPositionInRadius(playerPosition, radius);
        return GenerateRandomPosition(playerPosition, outerRadius, innerRadius);
    }

    public static Vector2 GetRandomPositionInRadius(Vector2 targetPosition, float radius)
    {
        PlayerScript playerScript = StageManager.Instance.CurrentPlayer;
        Vector2 playerPosition = playerScript.transform.position;

        float currentDegree = 360 / (float)(GameManager.Instance.Rand.NextDouble() % 360f);

        float xPosition = targetPosition.x + radius * Mathf.Cos(currentDegree);
        float yPosition = targetPosition.y + radius * Mathf.Sin(currentDegree);

        return new Vector2(xPosition, yPosition);
    }

    public static Vector2 GenerateRandomPosition(Vector2 center, float outerRadius, float innerRadius = 0)
    {
        float angle = Random.Range(0f, 360f); // Random angle in degrees

        // Convert angle to radians
        float radians = Mathf.Deg2Rad * angle;

        // Generate a random distance between the inner and outer radii
        float distance = Random.Range(innerRadius, outerRadius);

        // Calculate the random position
        float x = center.x + distance * Mathf.Cos(radians);
        float y = center.y + distance * Mathf.Sin(radians);

        return new Vector2(x, y);
    }

    public bool CheckIfIsInBounds(Vector2 position)
    {
        return position.x > _min.x && position.x < _max.x && position.y > _min.y && position.y < _max.y ? true : false;
    }

}
