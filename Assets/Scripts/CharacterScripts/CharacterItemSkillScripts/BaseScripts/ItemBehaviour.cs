using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    protected float _damage;
    protected float _deathTime;
    protected int _pierce;
    protected float _speed;
    protected float _knockback;

    protected int _itemLevel;

    protected delegate void LevelUPEffects();
    protected List<LevelUPEffects> _levelUPEffectsList;

    public void Awake()
    {
        _levelUPEffectsList = new List<LevelUPEffects>();
        _levelUPEffectsList.Add(Level2Effect);
        _levelUPEffectsList.Add(Level3Effect);
        _levelUPEffectsList.Add(Level4Effect);
        _levelUPEffectsList.Add(Level5Effect);
        _levelUPEffectsList.Add(Level6Effect);
        _levelUPEffectsList.Add(Level7Effect);
    }

    public void InitializeValue(float damage, float deathtime, int pierce, float speed, int level, float sizeScale, float knockback)
    {
        _damage = damage;
        _deathTime = deathtime;
        _pierce = pierce;
        _speed = speed;
        _itemLevel = level;
        _knockback = knockback;

        transform.localScale += new Vector3(sizeScale - 1f, sizeScale - 1f, 0);

        //LevelEffectCheck();

        Destroy(gameObject, _deathTime);
    }

    private void CheckPierce()
    {
        _pierce--;

        if( _pierce <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            GameObject enemy = collision.gameObject;
            EnemyScript script = enemy.GetComponent<EnemyScript>();

            //크리티컬 체크
            if (PlayerScript.Instance.CriticalCheck())
            {
                script.TakeCriticalDamage(PlayerScript.Instance.GetCritDamage(_damage));
            }
            else
            {
                script.TakeDamage(_damage);
            }

            if(_knockback != 0)
            {
                //do knockback
            }

            CheckPierce();
        }
    }

    protected virtual void LevelEffectCheck()
    {
        for(int i = _itemLevel - 2; i < _levelUPEffectsList.Count; i++)
        {
            _levelUPEffectsList[i]();
        }
    }

    protected virtual void Level2Effect()
    {
    }
    protected virtual void Level3Effect()
    {
    }
    protected virtual void Level4Effect()
    {
    }
    protected virtual void Level5Effect()
    {
    }
    protected virtual void Level6Effect()
    {
    }
    protected virtual void Level7Effect()
    {
    }
}
