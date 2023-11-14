using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    public void TakeDamage(float damage)
    {
        _hp -= damage;

        //적 피격 모션 : 간단하게 한 0.3초 동안 빨갛게 되기
        StartCoroutine(HitAnimation());

        //위에 데미지 뜨는 애니메이션
        GameObject DamagePopUp = Instantiate(GameManager.Instance.DamagePopUpPrefab, transform);
        TextMeshProUGUI tmp = DamagePopUp.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        tmp.text = damage.ToString();

        Rigidbody2D tmpRB = tmp.GetComponent<Rigidbody2D>();

        Vector2 direction = new Vector2(UnityEngine.Random.Range(-1,1), 1);
        tmpRB.AddForce(direction * 150);

        Destroy(DamagePopUp, 0.5f);
        


        CheckDeath();

    }

    private IEnumerator HitAnimation()
    {
        _spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.4f);
        _spriteRenderer.color = Color.white;
    }

    public void CheckDeath()
    {
        if (_hp <= 0)
        {
            StartCoroutine(KillEnemy());
        }
    }

    private IEnumerator KillEnemy()
    {
        StageManager.instance.UpdateKill();

        yield return new WaitForSeconds(0.1f);

        _movementSpeed = 0;
        Collider2D coll = GetComponent<Collider2D>();
        coll.enabled = false;
        Color originalColor = _spriteRenderer.color;

        float elapsedTime = 0f;

        while (elapsedTime < 0.5f)
        {
            float alpha = Mathf.Lerp(originalColor.a, 0f, elapsedTime / 0.5f);

            _spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        _spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);

        Instantiate(_expItemPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);

    }

}
