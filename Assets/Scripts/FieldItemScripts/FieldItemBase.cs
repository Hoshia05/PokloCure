using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class FieldItemBase : MonoBehaviour
{
    [SerializeField]
    protected SpriteRenderer _itemSprite;
    [SerializeField]
    public AudioClip _obtainSound;

    private float _movementSpeed = 20f;

    private float floatHeight = 0.1f; // Maximum float height
    private float floatSpeed = 2f; // Speed of floating
    private Vector2 startPos;

    protected bool isPicked = false;
    private Transform _playerTransform;

    private Coroutine _floatCoroutine;


    protected virtual void OnEnable()
    {
        startPos = _itemSprite.transform.localPosition;
        _floatCoroutine = StartCoroutine(FloatItem());
    }

    protected virtual void OnDisable()
    {
        isPicked = false;

    }

    protected virtual void Update()
    {
        TransformToPlayer();
    }

    public void PickedByPlayer(Transform PlayerTransform)
    {
        if (isPicked)
            return;

        isPicked = true;
        _playerTransform = PlayerTransform;

        // 둥둥 뜨는 코루틴은 줍는 순간 한 번만 멈춘다
        if (_floatCoroutine != null)
        {
            StopCoroutine(_floatCoroutine);
            _floatCoroutine = null;
        }

        _itemSprite.transform.localPosition = startPos;
    }

    public void TransformToPlayer()
    {
        if (isPicked)
        {
            // 플레이어가 파괴된 경우(사망) 따라가지 않는다
            if (_playerTransform != null)
                transform.position = Vector2.MoveTowards(transform.position, _playerTransform.position, _movementSpeed * Time.deltaTime);
        }
    }
    private IEnumerator FloatItem()
    {
        while (true)
        {
            float newY = startPos.y + (Mathf.Sin(Time.time * floatSpeed) + 1)  * floatHeight;
            _itemSprite.transform.localPosition = new Vector2(_itemSprite.transform.localPosition.x, newY);
            yield return null;
        }
    }
}
