using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FlailBehaviour : ItemBehaviour
{
    [SerializeField]
    private Animator _flailAnimation;

    public UnityEvent OnAnimationEnd;

    // 풀에서 꺼낼 때마다 호출된다 (예전 Start 의 내용)
    protected override void OnSpawned()
    {
        if(_flailAnimation != null)
            _flailAnimation.SetFloat("animSpeed", _speed);

    }

    public void FlailAnimationEnd()
    {
        OnAnimationEnd.Invoke();
    }

    public void FlailParentEnd()
    {
        DestroyProjectileImmediately();
    }

}
