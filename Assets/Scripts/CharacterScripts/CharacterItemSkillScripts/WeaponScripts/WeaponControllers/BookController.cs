using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookController : ItemController
{

    protected override void Launch()
    {
        base.Launch();

        float radius = 4f;
        float angleDifference = 360 / _projectileNum;

        for (int i = 0; i <_projectileNum; i++)
        {
            float x = Mathf.Sin(i * angleDifference * Mathf.Deg2Rad) * radius;
            float y = Mathf.Cos(i * angleDifference * Mathf.Deg2Rad) * radius;

            Vector2 initialPosition = new Vector2(x, y);

            BookBehaviour bookBehavior = (BookBehaviour)InstantiateProjectile();
            bookBehavior.SetInitialPosition(initialPosition, i * angleDifference);

        }

    }

    protected override void LevelUpEffect()
    {
        ResetProjectiles();
    }


    protected override void Level2Effect()
    {
        _additionalProjectiles++;
    }

    protected override void Level3Effect()
    {
        IncreaseDamagePercentage(0.4f);
        IncreaseDeathTime(1f);
        
    }

    protected override void Level4Effect()
    {
        _additionalProjectiles++;
    }
    protected override void Level5Effect()
    {
        IncreaseDamagePercentage(0.4f);
    }
    protected override void Level6Effect()
    {
        _additionalProjectiles++;
    }

    protected override void Level7Effect()
    {
        IncreaseDamagePercentage(0.4f);
    }
}
