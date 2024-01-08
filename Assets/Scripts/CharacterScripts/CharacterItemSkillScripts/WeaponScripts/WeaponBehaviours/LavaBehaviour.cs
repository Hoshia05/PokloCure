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


    private void Start()
    {
        _lavaCollider = GetComponent<Collider2D>();
        _rb = GetComponent<Rigidbody2D>();

        _lavaCollider.enabled = false;
        _lavaSprite.enabled = false;
        _bucketSprite.enabled = true;

        _hitCooldown = 0.75f;

        //launch towards random up direction

        float yDirection = Random.Range(0.1f, 1f); ;
        float xDirection = Random.Range(-0.5f, 0.5f);

        Vector2 launchDirection = new Vector2(xDirection, yDirection);

        _rb.AddForce(launchDirection * 1000);
        _rb.AddTorque(5f);

        StartCoroutine(TurnToLava());
    }

    IEnumerator TurnToLava()
    {
        StopCoroutine(DeathCoroutine);

        yield return new WaitForSeconds(1f);

        _rb.gravityScale = 0f;
        _rb.velocity = Vector2.zero;

        _lavaCollider.enabled = true;
        _lavaSprite.enabled = true;
        _bucketSprite.enabled = false;

        DeathCoroutine = StartCoroutine(DestroyProjectile(_deathTime));
    }


    void Update()
    {

    }

}
