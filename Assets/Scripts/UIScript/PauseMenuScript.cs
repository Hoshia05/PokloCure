using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuScript : MonoBehaviour
{

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

}
