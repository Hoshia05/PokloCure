using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class SpawnInfo
{
    public EnemyBase EnemyData;
    public float StartTime;
    public float EndTime;

    private bool _isBoss => EnemyData.isBossEnemy;

}

[CreateAssetMenu(fileName = "Unassigned SpawnProfile", menuName = "Scriptable Object/StageSpawnProfile")]

public class EnemySpawnProfileSO : ScriptableObject
{
    public List<SpawnInfo> enemies = new();

}

