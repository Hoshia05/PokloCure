using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEffectScript : MonoBehaviour
{
    // Start is called before the first frame update
    public void EndAnimation()
    {
        Destroy(gameObject);
    }
}
