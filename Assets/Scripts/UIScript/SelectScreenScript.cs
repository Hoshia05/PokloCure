using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectScreenScript : MonoBehaviour
{
    [SerializeField]
    private List<CharacterBase> _characterList = new();
    [SerializeField]
    private GameObject _gridSquarePrefab;


    public GameObject GridBase;

    // Start is called before the first frame update
    private void Awake()
    {
        foreach(CharacterBase character in _characterList)
        {
            GameObject charSquare = Instantiate(_gridSquarePrefab, GridBase.transform);
            GridSquareScript gridSquareScript = charSquare.GetComponent<GridSquareScript>();
            gridSquareScript.InitializeSquare(character);

        }
    }

}
