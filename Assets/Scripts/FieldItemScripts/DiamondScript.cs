using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiamondScript : FieldItemBase
{
    [SerializeField]
    private SpriteRenderer _spriteRenderer;

    private int _coinValue;

    public bool IsDebug;

    protected override void OnEnable()
    {
        base.OnEnable();

        ResetValue();
    }
    
    public void ResetValue()
    {
        if(GameManager.Instance.RollRandom(1))
        {
            _coinValue = 500;
            _spriteRenderer.color = Color.black;

        }
        else
        {
            _spriteRenderer.color = Color.white;
            _coinValue = GameManager.Instance.Rand.Next(3, 15);

        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StageManager.Instance.GainCoins(_coinValue);

            gameObject.SetActive(false);
        }
    }
}
