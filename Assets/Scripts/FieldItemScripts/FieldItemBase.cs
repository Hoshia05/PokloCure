using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class FieldItemBase : MonoBehaviour
{
    private float _movementSpeed = 20f;

    private bool isPicked = false;
    private Transform _playerTransform;

    protected virtual void Update()
    {
        TransformToPlayer();
    }

    public void PickedByPlayer(Transform PlayerTransform)
    {
        isPicked = true;
        _playerTransform = PlayerTransform;
    }

    public void TransformToPlayer()
    {
        if (isPicked)
            transform.position = Vector2.MoveTowards(transform.position, _playerTransform.position, _movementSpeed * Time.deltaTime);
    }
}
