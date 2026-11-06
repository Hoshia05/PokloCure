using System.Collections.Generic;
using System;

static class MyExtensions
{
    // GameManager가 없는 상황(에디터 테스트 등)을 위한 대체 난수
    private static readonly System.Random _fallbackRandom = new();

    // Fisher–Yates 셔플. 할당이 없고 리스트 크기에 제한이 없다.
    public static void Shuffle<T>(this IList<T> list)
    {
        System.Random rand = GameManager.Instance != null ? GameManager.Instance.Rand : _fallbackRandom;

        for (int n = list.Count - 1; n > 0; n--)
        {
            int k = rand.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
