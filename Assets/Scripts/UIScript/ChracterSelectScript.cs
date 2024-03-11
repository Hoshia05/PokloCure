using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChracterSelectScript : MonoBehaviour
{
    // Start is called before the first frame update
    public void SelectBlueTiger()
    {
        GameManager.Instance.SelectBlueTiger();
    }

    public void SelectGreenTiger()
    {
        GameManager.Instance.SelectGreenTiger();
    }
}
