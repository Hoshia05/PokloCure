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
        _descriptionText.text = "편곤은 호랑이에게 익숙한 무장입니다. 속도와 근거리 위주로 싸웁니다.";
    }

    public void UpdateRightSelect()
    {
        _descriptionText.text = "미국인에게 총기는 일상도구입니다. 공격력이 높은 원거리전 위주로 싸웁니다.";
    }
}
