using System;
using System.Collections.Generic;

/// <summary>
/// 가중치 기반 랜덤 선택 (유니티 의존성 없음 → EditMode 테스트 가능).
/// </summary>
public static class WeightedPick
{
    /// <summary>
    /// weights 중 양수인 항목만 대상으로 가중치 비례 선택해 그 인덱스를 반환한다.
    /// 선택 가능한 항목이 없으면 -1.
    /// nextInt(exclusiveMax)는 [0, exclusiveMax) 범위의 정수를 돌려줘야 한다.
    /// </summary>
    public static int PickIndex(IReadOnlyList<int> weights, Func<int, int> nextInt)
    {
        if (weights == null || nextInt == null)
            return -1;

        int totalWeight = 0;
        for (int i = 0; i < weights.Count; i++)
        {
            if (weights[i] > 0)
                totalWeight += weights[i];
        }

        if (totalWeight <= 0)
            return -1;

        int roll = nextInt(totalWeight);

        for (int i = 0; i < weights.Count; i++)
        {
            if (weights[i] <= 0)
                continue;

            if (roll < weights[i])
                return i;

            roll -= weights[i];
        }

        return -1;
    }
}
