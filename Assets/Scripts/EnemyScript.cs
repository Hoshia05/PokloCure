﻿using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    // Start is called before the first frame update

    private GameObject _playerCharacter;
    private Vector2 _playerPosition;

    private Vector2 _enemyLineOfSight;

    [SerializeField]
    private GameObject _expItemPrefab;

    [Header("캐릭스펙")]
    [SerializeField]
    private float _movementSpeed;
    [SerializeField]
    private float _attackCoolTime;
    [SerializeField]
    private float _defensePoints;
    [SerializeField]
    private float _hp;
    [SerializeField]
    private float _attackMultiplier;

    [SerializeField]
    private SpriteRenderer _spriteRenderer;


    private void Awake()
    {
        _playerCharacter = GameObject.FindGameObjectWithTag("Player");
        _hp = 9;
    }

    // Update is called once per frame
    void Update()
    {
        _playerPosition = _playerCharacter.transform.position;
        Movement();
        UpdateLOS();
    }

    void Movement()
    {
        float step = _movementSpeed * Time.deltaTime;
        transform.position = Vector2.MoveTowards(transform.position, _playerCharacter.transform.position, step);

    }

    void UpdateLOS()
    {
        _enemyLineOfSight = (_playerPosition - (Vector2)transform.position).normalized;

        _spriteRenderer.flipX = _enemyLineOfSight.x > 0 ? true : false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameObject player = collision.gameObject;
            PlayerScript script = player.GetComponent<PlayerScript>();
            script.TakeDamage(15);
        }
    }

    public void Damage(float damage)
    {
        _hp -= damage;
        CheckDeath();

    }

    public void CheckDeath()
    {
        if (_hp <= 0)
        {

            Instantiate(_expItemPrefab, transform.position, Quaternion.identity);

            Destroy(gameObject);
        }
    }
}
