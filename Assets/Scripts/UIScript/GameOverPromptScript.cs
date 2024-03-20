using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOverPromptScript : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _scoreText;

    public void ResumeButton()
    {
        StageManager.Instance.PauseMenu();
    }

    public void TitleScreenButton()
    {
        Time.timeScale = 1;
        GameManager.Instance.MoveToTitleScene();
    }

    public void QuitGameButton()
    {
        Time.timeScale = 1;
        GameManager.Instance.QuitGame();
    }

    public void SetScore(int score)
    {
        _scoreText.text = score.ToString();
    }
}
