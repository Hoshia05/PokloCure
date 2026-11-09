using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEffectScript : MonoBehaviour
{
    // 애니메이션 이벤트에서 호출된다.
    // ShockwaveEffect 프리팹은 루트와 자식에 이 스크립트가 모두 붙어 있다.
    // 자식만 끄면 풀로 반환되지 않고 자식이 계속 비활성으로 남으므로,
    // 풀에서 꺼낸 오브젝트(PooledObject)가 부모 쪽에 있으면 그쪽을 반환한다.
    public void EndAnimation()
    {
        PooledObject pooled = GetComponentInParent<PooledObject>(true);

        if (pooled != null)
            pooled.Release();
        else
            gameObject.SetActive(false);
    }
}
