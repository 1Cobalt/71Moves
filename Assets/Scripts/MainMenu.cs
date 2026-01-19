using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject panelMenu;
    public GameObject panelSettings;
    public GameObject panelInfo1;
    public GameObject panelInfo2;
    public GameObject panelWin;
    public GameObject panelLose;
    public GameObject inGameObjects;

    void Start()
    {
        Time.timeScale = 1.0f;
    }

    public void StartGame1()
    {
        SceneManager.LoadScene("PVE");
    }
    public void StartGame2()
    {
        SceneManager.LoadScene("PVP");
    }

    public void Pause()
    {
      
        panelMenu.SetActive(true);
        inGameObjects.SetActive(false);

    }
    public void UnPause()
    {
        panelMenu.SetActive(false);
        inGameObjects.SetActive(true);
    }

    public void OpenSettings()
    {
        
        panelSettings.SetActive(true);
        panelMenu.SetActive(false);
    }

    public void CloseSettings()
    {
   
        panelSettings.SetActive(false);
        panelMenu.SetActive(true);
    }

    public void OpenInfo1()
    {
        Time.timeScale = 0.0f;
        panelInfo1.SetActive(true);
        panelMenu.SetActive(false);
    }


    public void CloseInfo1()
    {
        Time.timeScale = 1.0f;
        panelInfo1.SetActive(false);
        panelMenu.SetActive(true);
    }

    public void CloseInfo2()
    {
        Time.timeScale = 1.0f;
        panelInfo2.SetActive(false);
        panelInfo1.SetActive(true);
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }



    public void FinishGame(int player1Score, int player2Score)
    {
        panelMenu.SetActive(false);
        if (player1Score > player2Score)
        {
            panelWin.SetActive(true);
        }
        else 
        {
            panelLose.SetActive(true);
        }
      
    }

    public void OpenMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}