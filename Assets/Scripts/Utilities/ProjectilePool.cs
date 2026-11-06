using System.Collections.Generic;
using UnityEngine;

// 플레이어 투사체 풀. 프리팹 하나당 SimplePool 하나를 두고 Instantiate/Destroy 대신 재사용한다.
// 꺼낼 때 부모 / 위치 / 회전 / 크기 / Rigidbody 속도를 프리팹의 초기 상태로 되돌린다.
public class ProjectilePool : MonoBehaviour
{
    private static ProjectilePool _instance;

    public static ProjectilePool Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameObject("ProjectilePool").AddComponent<ProjectilePool>();
            }

            return _instance;
        }
    }

    private class Entry
    {
        public SimplePool Pool;
        public Vector3 LocalPosition;
        public Quaternion LocalRotation;
        public Vector3 LocalScale;
    }

    private readonly Dictionary<GameObject, Entry> _entries = new();

    private void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
    }

    private Entry GetEntry(GameObject prefab)
    {
        if (_entries.TryGetValue(prefab, out Entry entry))
            return entry;

        entry = new Entry
        {
            // 상한 없음: 예전 Instantiate 방식과 같이 필요한 만큼 만든다
            Pool = new SimplePool(prefab, transform, 0, int.MaxValue, OnCreated, true),
            LocalPosition = prefab.transform.localPosition,
            LocalRotation = prefab.transform.localRotation,
            LocalScale = prefab.transform.localScale,
        };

        _entries.Add(prefab, entry);
        return entry;
    }

    private static void OnCreated(GameObject go)
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        ItemController.DebugCreateCount++;
#endif
    }

    // parent 가 null 이면 월드에 독립적으로 놓는다 (worldPosition, 회전 없음).
    // parent 가 있으면 Instantiate(prefab, parent) 와 같이 프리팹의 로컬 위치/회전으로 붙인다.
    public GameObject Get(GameObject prefab, Transform parent, Vector3 worldPosition)
    {
        Entry entry = GetEntry(prefab);
        GameObject go = entry.Pool.Get();
        Transform t = go.transform;

        if (parent != null)
        {
            t.SetParent(parent, false);
            t.localPosition = entry.LocalPosition;
            t.localRotation = entry.LocalRotation;
        }
        else
        {
            t.SetParent(null, false);
            t.SetPositionAndRotation(worldPosition, Quaternion.identity);
        }

        t.localScale = entry.LocalScale;

        go.SetActive(true);

        if (go.TryGetComponent(out Rigidbody2D rb))
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        return go;
    }
}
