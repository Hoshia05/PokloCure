using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChracterSelectionDescriptionScript : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _descriptionText;

    private void Start()
    {
        UpdateDefault();
    }

    public void UpdateDefault()
    {
        _descriptionText.text = "호랭이를 선택하세요";
    }

    public void UpdateLeftSelect()
    {
        _descriptionText.text = "왼쪽 설명";
    }

    public void UpdateRightSelect()
    {
        _descriptionText.text = "오른쪽 설명";
    }
}
