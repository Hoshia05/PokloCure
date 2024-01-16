using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class PatternSpawnInfo
{
    public EnemyBase EnemyData;
    public float SpawnTime;
    public SpawnType SpawnType;
    public int AmountSpawned;

}

public enum SpawnType
{
    CLUSTER,
    HORDE,
    WALL,
    STAMPEDE,
    RING,

}

[CreateAssetMenu(fileName = "Unassigned PatternSpawnProfile", menuName = "Scriptable Object/StagePatternSpawnProfile")]

public class EnemyPatternSpawnProfileSO : ScriptableObject
{
    public List<PatternSpawnInfo> patterns = new();

}

