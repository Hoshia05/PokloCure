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
    [SerializeField] AudioClip _hoverSound;
    [SerializeField] AudioClip _selectSound;

    private Color _blueTigerInitialColor;
    private Color _greenTigerInitialColor;

    public void Start()
    {
        _blueTigerInitialColor = _blueTigerSprite.color;
        _greenTigerInitialColor = _greenTigerSprite.color;
    }

    public void SelectBlueTiger()
    {
        SoundFXManager.Instance.PlaySoundFXClip(_selectSound, transform, 1f);
        GameManager.Instance.SelectBlueTiger();
    }

    public void SelectGreenTiger()
    {
        SoundFXManager.Instance.PlaySoundFXClip(_selectSound, transform, 1f);
        GameManager.Instance.SelectGreenTiger();
    }

    public void HoverOnBlueTiger()
    {
        SoundFXManager.Instance.PlaySoundFXClip(_hoverSound, transform, 1f);
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
        SoundFXManager.Instance.PlaySoundFXClip(_hoverSound, transform, 1f);
        _greenTigerAnimation.SetBool("Select", true);
        _greenTigerSprite.color = Color.white;
    }

    public void HoverOffGreenTiger()
    {
        _greenTigerAnimation.SetBool("Select", false);
        _greenTigerSprite.color = _greenTigerInitialColor;
    }

}
