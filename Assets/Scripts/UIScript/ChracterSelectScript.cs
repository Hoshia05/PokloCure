using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChracterSelectScript : MonoBehaviour
{
    [SerializeField] Animator _blueTigerAnimation;
    [SerializeField] Image _blueTigerSprite;
    [SerializeField] Animator _greenTigerAnimation;
    [SerializeField] Image _greenTigerSprite;

    private Color _blueTigerInitialColor;
    private Color _greenTigerInitialColor;

    public void Start()
    {
        _blueTigerInitialColor = _blueTigerSprite.color;
        _greenTigerInitialColor = _greenTigerSprite.color;
    }

    public void SelectBlueTiger()
    {
        GameManager.Instance.SelectBlueTiger();
    }

    public void SelectGreenTiger()
    {
        GameManager.Instance.SelectGreenTiger();
    }

    public void HoverOnBlueTiger()
    {
        _blueTigerAnimation.SetBool("Select", true);
        _blueTigerSprite.color = Color.white;

    }

    public void HoverOffBlueTiger()
    {
        _blueTigerAnimation.SetBool("Select", false);
        _blueTigerSprite.color = _blueTigerInitialColor;
    }


    public void HoverOnGreenTiger()
    {
        _greenTigerAnimation.SetBool("Select", true);
        _greenTigerSprite.color = Color.white;
    }

    public void HoverOffGreenTiger()
    {
        _greenTigerAnimation.SetBool("Select", false);
        _greenTigerSprite.color = _greenTigerInitialColor;
    }

}
