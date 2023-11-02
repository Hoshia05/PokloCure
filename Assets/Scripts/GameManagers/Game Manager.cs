using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public List<GameObject> EnemyList;
    public GameObject PlayerCharacterPrefab;

    [Header("µð¹ö±ë¿ë")]
    [SerializeField]
    private CharacterBase _selectedCharacter;

    public CharacterBase SelectedCharacter
    {
        get 
        {
            return _selectedCharacter;
        }
        set
        {
            _selectedCharacter = value;
        }
    }

    private static string _selectedCharacterName;

    private void Awake()
    {
        if(Instance == null)
            Instance = this;

        DontDestroyOnLoad(gameObject);
    }

}
