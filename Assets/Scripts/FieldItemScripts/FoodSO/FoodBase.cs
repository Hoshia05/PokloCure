using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Unassigned Food", menuName = "Scriptable Object/FoodBase")]
public class FoodBase : ScriptableObject
{

    public float HealValue = 1f;
    public bool IsBasicBurger = false;
    public Sprite FoodSprite;


}
