using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FlailHelperScript : MonoBehaviour
{
    [SerializeField]
    private FlailBehaviour _flailBehaviour;

    public UnityEvent OnAnimationEnd;


    public void FlailAnimationEnd()
    {
        OnAnimationEnd.Invoke();
    }

    public void FlailParentEnd()
    {
        _flailBehaviour.DestroyProjectileImmediately();
    }
}
