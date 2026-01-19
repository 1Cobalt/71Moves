using UnityEngine;
using MyDice.Board;
[System.Serializable]
public class PlayerData
{
    public PlayerMode playerMode;
    public PlayerColor playerColor;
    public bool enable = false;
    public void Load(string key)
    {
        if (PlayerPrefs.HasKey(key + "playerMode"))
        {
            playerMode = (PlayerMode)PlayerPrefs.GetInt(key + "playerMode");
        }
        if (PlayerPrefs.HasKey(key + "playerColor"))
        {
            playerColor = (PlayerColor)PlayerPrefs.GetInt(key + "playerColor");
        }
        if (PlayerPrefs.HasKey(key + "enable"))
        {
            enable = PlayerPrefs.GetInt(key + "enable") > 0;
        }
    }
    public void Save(string key)
    {
        PlayerPrefs.SetInt(key + "playerMode", (int)playerMode);
        PlayerPrefs.SetInt(key + "playerColor", (int)playerColor);
        PlayerPrefs.SetInt(key + "enable", enable ? 1 : 0);

        PlayerPrefs.Save();
    }
}
