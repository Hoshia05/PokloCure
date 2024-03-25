using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class ChaseState: IEnemyState
{
    private EnemyFSM _enemyFSM;
    private EnemyScript _enemyScript;

    public ChaseState(EnemyFSM fsm, EnemyScript enemyScript)
    {
        _enemyFSM = fsm;
        _enemyScript = enemyScript;
    }

    public void EnterState()
    {

    }

    public void UpdateState()
    {
        float distance = Vector2.Distance(_enemyFSM.transform.position, StageManager.Instance.CurrentPlayer.transform.position);

        if(distance < _enemyScript.RangedAttackDistance)
        {
            _enemyFSM.TransitionToState(_enemyFSM.AttackState);
        }
        else if(!_enemyScript.IsPatternSpawn && !_enemyScript.IsStunned)
        {
            float step = _enemyScript.CurrentMovementSpeed * Time.deltaTime;
            _enemyFSM.transform.position = Vector2.MoveTowards(_enemyFSM.transform.position, StageManager.Instance.CurrentPlayer.transform.position, step);
        }
    }

    public void ExitState()
    {

    }
}

public class AttackState : IEnemyState
{
    private EnemyFSM _enemyFSM;
    private EnemyScript _enemyScript;
    private float _attackDuration;
    private float _timer;

    public AttackState(EnemyFSM fsm, EnemyScript enemyScript)
    {
        _enemyFSM = fsm;
        _enemyScript = enemyScript;
        _attackDuration = _enemyScript.RangedAttackCooltime;
    }

    public void EnterState()
    {
        _timer = 0;
        _enemyScript.ConductRangedAttack();
    }

    public void UpdateState()
    {
        _timer += Time.deltaTime;

        if (_timer >= _attackDuration)
        {
            // Transition back to chase state
            _enemyFSM.TransitionToState(_enemyFSM.ChaseState);
        }
    }

    public void ExitState()
    {

    }
}
