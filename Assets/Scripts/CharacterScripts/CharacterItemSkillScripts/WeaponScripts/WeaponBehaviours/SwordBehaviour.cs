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

    public void TurnOffCollider()
    {
        _collider.enabled = false;
    }

    public void Terminate()
    {
        DestroyProjectileImmediately();
    }
}
