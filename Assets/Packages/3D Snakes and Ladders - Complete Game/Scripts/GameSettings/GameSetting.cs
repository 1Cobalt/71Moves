using UnityEngine;

[System.Serializable]
public class GameSetting
{
    public DiceMode diceMode;
    public PlayerData[] players;
    public GameSetting()
    {
        players = new PlayerData[4];
    }
    public void Load()
    {
        if (PlayerPrefs.HasKey("diceMode"))
        {
            diceMode = (DiceMode)PlayerPrefs.GetInt("diceMode");
        }
        if (players != null)
        {
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i] == null)
                    players[i] = new PlayerData();
                players[i].Load(i.ToString());
            }
        }
    }
    public void Save()
    {
        PlayerPrefs.SetInt("diceMode", (int)diceMode);
        PlayerPrefs.Save();
    }
    public void Flush()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }
}
