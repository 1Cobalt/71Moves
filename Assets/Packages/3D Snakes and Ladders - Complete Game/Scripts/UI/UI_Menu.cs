using UnityEngine;
using MyDice;
using UnityEngine.SceneManagement;

public class UI_Menu : MonoBehaviour
{
    [HideInInspector]
    public static int levelId = 1;
    private void Start()
    {
        Time.timeScale = 1;
        var gameSetting = new GameSetting();
        gameSetting.Flush();
    }

    public void WebPage(string url)
    {
        Application.OpenURL(url);
    }

    #region select level id
    public void SetLevel(int id)
    {
        levelId = id;
    }
    #endregion
    public void Play()
    {
        SceneManager.LoadScene("Level_" + levelId.ToString());
    }
    public void QuitGame()
    {
        GameSetting gameSetting = new GameSetting();
        gameSetting.Flush();
        Extensions.Quit();
    }
}
