using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScript : MonoBehaviour
{

    [SerializeField]
    private PlayerScript _playerCharacter; 
    // Start is called before the first frame update
    void Start()
    {
        GameObject PlayerObject = GameObject.FindGameObjectWithTag("Player");
        _playerCharacter = PlayerObject.GetComponent<PlayerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
