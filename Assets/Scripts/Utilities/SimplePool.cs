using System;
using System.Collections.Generic;
using UnityEngine;

// 프리팹 하나당 하나씩 쓰는 간단한 오브젝트 풀.
// 비활성 오브젝트를 선형 탐색하지 않고, 스택(free list)에서 O(1)로 꺼낸다.
public class SimplePool
{
    private readonly GameObject _prefab;
    private readonly Transform _parent;
    private readonly Action<GameObject> _onCreate;
    private readonly int _maxTotal;
    private readonly bool _reparentOnRelease;
    private readonly Stack<PooledObject> _free = new();
    private int _totalCreated;

    // prewarm   : 미리 만들어 둘 개수
    // maxTotal  : 풀이 만들 수 있는 최대 개수 (넘으면 Get()이 null을 반환)
    // onCreate  : 새 인스턴스를 만들 때 1회 호출 (비활성화되기 직전)
    // reparentOnRelease : 반환할 때 다시 풀의 parent 아래로 옮긴다 (꺼낼 때 다른 부모에 붙여 쓰는 경우)
    public SimplePool(GameObject prefab, Transform parent, int prewarm, int maxTotal = int.MaxValue, Action<GameObject> onCreate = null, bool reparentOnRelease = false)
    {
        _prefab = prefab;
        _parent = parent;
        _maxTotal = maxTotal;
        _onCreate = onCreate;
        _reparentOnRelease = reparentOnRelease;

        for (int i = 0; i < prewarm && _totalCreated < _maxTotal; i++)
        {
            _free.Push(Create());
        }
    }

    private PooledObject Create()
    {
        GameObject go = UnityEngine.Object.Instantiate(_prefab, _parent);

        PooledObject pooled = go.AddComponent<PooledObject>();
        pooled.Pool = this;
        pooled.InPool = true;
        _totalCreated++;

        _onCreate?.Invoke(go);

        go.SetActive(false);
        return pooled;
    }

    // 비활성 상태의 오브젝트를 반환한다. 활성화는 호출한 쪽에서 한다.
    // 최대 개수에 도달해서 더 줄 수 없으면 null.
    public GameObject Get()
    {
        PooledObject pooled;

        if (_free.Count > 0)
            pooled = _free.Pop();
        else if (_totalCreated < _maxTotal)
            pooled = Create();
        else
            return null;

        pooled.InPool = false;
        return pooled.gameObject;
    }

    public void Release(PooledObject pooled)
    {
        // 이미 풀에 있는 오브젝트를 두 번 반환하면 스택에 중복으로 쌓이므로 막는다.
        if (pooled == null || pooled.InPool)
            return;

        pooled.InPool = true;

        if (_reparentOnRelease && pooled.transform.parent != _parent)
            pooled.transform.SetParent(_parent, false);

        pooled.gameObject.SetActive(false);
        _free.Push(pooled);
    }
}

public class PooledObject : MonoBehaviour
{
    [NonSerialized] public SimplePool Pool;
    [NonSerialized] public bool InPool;

    public void Release()
    {
        if (Pool != null)
            Pool.Release(this);
        else
            gameObject.SetActive(false);
    }

    // 풀에서 온 오브젝트면 풀로 반환하고, 아니면 그냥 비활성화한다.
    public static void ReleaseOrDeactivate(GameObject go)
    {
        if (go.TryGetComponent(out PooledObject pooled))
            pooled.Release();
        else
            go.SetActive(false);
    }
}
