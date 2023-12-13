using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossScript : EnemyScript
{
    // Start is called before the first frame update
    public override void CheckDeath()
    {
        if (_currentMaxHP <= 0)
        {
            StartCoroutine(KillBoss());
        }
    }

    protected IEnumerator KillBoss()
    {
        StageManager.instance.UpdateKill();
        Collider2D coll = GetComponent<Collider2D>();
        coll.enabled = false;

        yield return new WaitForSeconds(0.1f);

        _currentMovementSpeed = 0;
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

        GameObject expItem = Instantiate(GameManager.Instance.ExpItemPrefab, RandomNearPosition(), Quaternion.identity);
        ExpItemScript expItemScript = expItem.GetComponent<ExpItemScript>();
        expItemScript.SetExpValue(_dropExpValue);

        //버거소환
        DropBurger();
        //다이아소환
        DropDiamond();
        //아이템상자 소환
        Instantiate(GameManager.Instance.TreasureBoxPrefab, RandomNearPosition(), Quaternion.identity);

        Destroy(gameObject);
        StageManager.instance.CurrentEnemyCount--;

    }
}
