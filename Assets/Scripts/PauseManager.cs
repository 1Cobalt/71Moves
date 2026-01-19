using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{

    public static PauseManager Instance;

    public GameObject pauseScreen;      // Панель паузы
    public GameObject inGameHud;

    private void Awake()
    {
        Instance = this;

        if (pauseScreen != null)
            pauseScreen.SetActive(false);
    }

    public void Pause()
    {
        pauseScreen.SetActive(true);
        inGameHud.SetActive(false);
    }

    public void UnPause()
    {
        pauseScreen.SetActive(false);
        inGameHud.SetActive(true);
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
