using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UI_Game : MonoBehaviour
{
    bool isPause;
    private void Start()
    {
        Time.timeScale = 1;
    }
    public void PauseGame ()
    {
        if (!isPause)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
        isPause = !isPause;
    }

    public void RestartGame()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    public void HomePage ()
    {
        SceneManager.LoadScene(0);
    }

}
