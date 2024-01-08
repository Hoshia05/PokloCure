using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepController : ItemController
{
    private float _regenTime = 10f;
    private float _dropChance = 10f;

    private void Start()
    {
        ApplyStats();
    }


    protected override void LevelUpEffect()
    {
        ApplyStats();
    }

    protected override void ApplyStats()
    {
        _currentCooldownDuration = _regenTime;
        _buff.BurgerDropChance = _dropChance;
        UpdateBuff();
    }

    protected override void Launch()
    {
        base.Launch();
        SummonBurger();

    }

    void SummonBurger()
    {
        Instantiate(GameManager.Instance.BurgerPrefab, RandomNearPosition(), Quaternion.identity);
    }

    private Vector2 RandomNearPosition()
    {
        return new Vector2(transform.position.x + GameManager.Instance.Rand.Next(1, 10) * 0.1f, transform.position.y + GameManager.Instance.Rand.Next(1, 10) * 0.1f);
    }

    protected override void Level2Effect()
    {
        _regenTime = 9f;
        _dropChance = 12f;
    }
    protected override void Level3Effect()
    {
        _regenTime = 8f;
        _dropChance = 15f;
    }
    protected override void Level4Effect()
    {
        _regenTime = 7f;
        _dropChance = 18f;
    }
    protected override void Level5Effect()
    {
        _regenTime = 6f;
        _dropChance = 20f;
    }
}
