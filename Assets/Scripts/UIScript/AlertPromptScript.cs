using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AlertPromptScript : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _promptText;

    public void ChangePrompt(string newText)
    {
        _promptText.text = newText;
    }
}
