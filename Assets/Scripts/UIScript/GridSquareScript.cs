using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GridSquareScript : MonoBehaviour
{
    [SerializeField]
    private Image _gridImage;
    private CharacterBase _characterData;

    private void Awake()
    {
    }

    public void InitializeSquare(CharacterBase CharData)
    {
        _characterData = CharData;
        _gridImage.sprite = CharData.CharacterPortrait;

    }
    
    public void CharacterSelect()
    {
        GameManager.Instance.SelectedCharacter = _characterData;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

}
