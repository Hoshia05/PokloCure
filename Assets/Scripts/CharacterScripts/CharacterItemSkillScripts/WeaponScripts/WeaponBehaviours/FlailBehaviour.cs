using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FlailBehaviour : ItemBehaviour
{
    [SerializeField]
    private Animator _flailAnimation;

    public UnityEvent OnAnimationEnd;

    private void Start()
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
