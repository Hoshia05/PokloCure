using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FoodItemScript : FieldItemBase
{
    [SerializeField]
    private FoodBase _foodData;

    private void Awake()
    {
        _itemSprite.sprite = _foodData.FoodSprite;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameObject player = collision.gameObject;
            PlayerScript script = player.GetComponent<PlayerScript>();
            script.HealHP(_foodData.HealValue, _foodData.IsBasicBurger);

            Destroy(gameObject);
        }
    }
}
