using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class EnemyFSM : MonoBehaviour
{
    private EnemyScript _enemyScript;
    private IEnemyState currentState;

    public IEnemyState ChaseState;
    public IEnemyState AttackState;

    private void Awake()
    {
        _enemyScript = GetComponent<EnemyScript>();
    }

    private void Start()
    {
        ChaseState = new ChaseState(this, _enemyScript);
        AttackState = new AttackState(this, _enemyScript);

        currentState = ChaseState;
    }

    private void Update()
    {
        // Update current state
        currentState.UpdateState();
    }

    public void TransitionToState(IEnemyState nextState)
    {
        // Transition to the new state
        currentState.ExitState();
        currentState = nextState;
        currentState.EnterState();
    }
}
