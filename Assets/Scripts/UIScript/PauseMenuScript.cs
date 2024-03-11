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
        GameManager.Instance.MoveToTitleScene();
    }

    public void QuitGameButton()
    {
        GameManager.Instance.QuitGame();
    }

}
