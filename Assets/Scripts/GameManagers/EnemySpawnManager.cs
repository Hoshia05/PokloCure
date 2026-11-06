using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour 
{
    public static EnemySpawnManager Instance;

    private Vector2 _min;
    private Vector2 _max;

    private void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void InitializeMinMax(Vector2 min, Vector2 max)
    {
        _min = min;
        _max = max;
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
        float a = Random.Range(0f, Mathf.PI * 2f);

        return targetPosition + new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * radius;
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

    // 맵 안에 들어오는 위치를 제한된 횟수만 시도해서 찾는다 (무한 루프 방지)
    public bool TryGetPositionInPlayerRing(float outer, float inner, out Vector2 pos, int maxAttempts = 20)
    {
        for (int i = 0; i < maxAttempts; i++)
        {
            pos = GetRandomPositionInPlayerRadius(outer, inner);
            if (CheckIfIsInBounds(pos))
                return true;
        }

        pos = default;
        return false;
    }

    public bool CheckIfIsInBounds(Vector2 position)
    {
        return position.x > _min.x && position.x < _max.x && position.y > _min.y && position.y < _max.y ? true : false;
    }

}
