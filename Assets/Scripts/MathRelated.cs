using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathRelated
{
    public static float GetNextExpRequirement(int currentLevel)
    {
        double nextExp = 4 * Math.Pow((double)(currentLevel + 1), 2.1) - (4 / Math.Pow((double)(currentLevel + 1), 2.1));

        return (float)nextExp;
    }

}
