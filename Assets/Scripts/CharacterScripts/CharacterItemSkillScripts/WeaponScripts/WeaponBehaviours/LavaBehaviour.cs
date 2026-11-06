using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class LavaBehaviour : ItemBehaviour
{
    [SerializeField]
    private SpriteRenderer _bucketSprite;
    [SerializeField]
    private SpriteRenderer _lavaSprite;

    private Collider2D _lavaCollider;
    private Rigidbody2D _rb;
    private float _baseGravityScale;

    private void Awake()
    {
        _lavaCollider = GetComponent<Collider2D>();
        _rb = GetComponent<Rigidbody2D>();
        _baseGravityScale = _rb.gravityScale;
    }

    // 풀에서 꺼낼 때마다 호출된다 (예전 Start 의 내용 + 재사용을 위한 상태 복원)
    protected override void OnSpawned()
    {
        _rb.gravityScale = _baseGravityScale;
        _rb.linearVelocity = Vector2.zero;
        _rb.angularVelocity = 0f;

        _lavaCollider.enabled = false;
        _lavaSprite.enabled = false;
        _bucketSprite.enabled = true;

        _hitCooldown = 0.75f;

        //launch towards random up direction

        float yDirection = Random.Range(1f, 2f); ;
        float xDirection = Random.Range(-0.5f, 0.5f);

        Vector2 launchDirection = new Vector2(xDirection, yDirection);

        _rb.AddForce(launchDirection * 500);
        _rb.AddTorque(5f);

        StartCoroutine(TurnToLava());
    }

    IEnumerator TurnToLava()
    {
        StopCoroutine(DeathCoroutine);

        yield return new WaitForSeconds(1f);

        _rb.gravityScale = 0f;
        _rb.linearVelocity = Vector2.zero;

        _lavaCollider.enabled = true;
        _lavaSprite.enabled = true;
        _bucketSprite.enabled = false;

        DeathCoroutine = StartCoroutine(DestroyProjectile(_deathTime));
    }
}
