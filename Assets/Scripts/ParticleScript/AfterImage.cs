using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterImage : MonoBehaviour
{
    private ParticleSystem ps;

    private void Start()
    {
        ps = GetComponentInChildren<ParticleSystem>();
    }

    private void Update()
    {
        if (ps != null)
        {
            ParticleSystem.MainModule main = ps.main;

            if (main.startRotation.mode == ParticleSystemCurveMode.Constant)
            {
                main.startRotation = -transform.eulerAngles.z * Mathf.Deg2Rad;
            }
        }
    }
}
