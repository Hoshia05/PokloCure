using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public List<GameObject> EnemyList;

    private CharacterBase _selectedCharacter;

    public CharacterBase SelectedCharacter
    {
        get 
        {
            return _selectedCharacter;
        }
        private set
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

    // Start is called before the first frame update
    void Start()
    {
        
    }

}
