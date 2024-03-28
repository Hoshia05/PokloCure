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
        _descriptionText.text = "호랑이 영물이였던 시절의 경험을 살려 편곤을 휘두르며 근거리 위주로 싸웁니다";
    }

    public void UpdateRightSelect()
    {
        _descriptionText.text = "미국에서 총기와 친했던 경험을 살려 권총을 쏘며 원거리 위주로 싸웁니다 ";
    }
}
