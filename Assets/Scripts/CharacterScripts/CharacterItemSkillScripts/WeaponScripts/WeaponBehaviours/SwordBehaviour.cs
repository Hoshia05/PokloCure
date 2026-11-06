using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordBehaviour : ItemBehaviour
{
    private Collider2D _collider;

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
    }

    // 애니메이션 이벤트가 꺼 둔 콜라이더를 재사용할 때 다시 켠다
    protected override void OnSpawned()
    {
        _collider.enabled = true;
    }

    public void TurnOffCollider()
    {
        _collider.enabled = false;
    }

    public void Terminate()
    {
        DestroyProjectileImmediately();
    }
}
