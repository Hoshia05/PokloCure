using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class FieldItemBase : MonoBehaviour
{
    // Update is called once per frame
    private float _movementSpeed = 20f;

    private bool isPicked = false;
    private Transform _playerTransform;

    private void Update()
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