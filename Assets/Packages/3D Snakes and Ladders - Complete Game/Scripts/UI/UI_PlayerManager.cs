using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UI_PlayerManager : MonoBehaviour
{
    #region variable
    public PlayerIndex playerIndex;
    public PlayerColor playerColor;
    [Header("Player Sprite(s)")]
    public Sprite playerRedUI;
    public Sprite playerGreenUI;
    public Sprite playerBlueUI;
    public Sprite playerYellowUI;
    [Header("UI")]
    public Image icon;
    public GameObject dropdown;
    public Button AddPlayerButton;
    [HideInInspector] public PlayerData playerData;
    private Dropdown _dropdown;
    private Image _dropdownImage;

    #endregion
    #region Functions
    private void OnEnable()
    {
        _dropdown = dropdown.GetComponent<Dropdown>();
        _dropdownImage = dropdown.GetComponent<Image>();
    }
    private void Start()
    {
        playerData = new PlayerData();
        //playerData.Load(((int)playerIndex).ToString());
        //inputField.text = playerData.name;
        //_dropdown.value = (playerData.playerMode == MyDice.Board.PlayerMode.Human) ? 0 : 1;
        playerData.enable = true;
        playerData.playerColor = playerColor;
        //this.gameObject.SetActive(playerData.enable);
        changeColor(playerColor);
    }
    private void Update()
    {
        bool saving = false;
        #region dropdown
        if (_dropdown.value == 0)
        {
            if (playerData.playerMode != MyDice.Board.PlayerMode.Human)
            {
                playerData.playerMode = MyDice.Board.PlayerMode.Human;
                saving = true;
            }
        }
        else
        {
            if (playerData.playerMode != MyDice.Board.PlayerMode.CPU)
            {
                playerData.playerMode = MyDice.Board.PlayerMode.CPU;
                saving = true;
            }
        }
        #endregion
        if (saving)
        {
            playerData.Save(((int)playerIndex).ToString());
        }
    }
    #endregion
    #region functions
    #region color
    public void changeColor_toRed()
    {
        updateFinalColor(PlayerColor.Red);
        changeColor(PlayerColor.Red);
    }
    public void changeColor_toGreen()
    {
        updateFinalColor(PlayerColor.Green);
        changeColor(PlayerColor.Green);
    }
    public void changeColor_toBlue()
    {
        updateFinalColor(PlayerColor.Blue);
        changeColor(PlayerColor.Blue);
    }
    public void changeColor_toYellow()
    {
        updateFinalColor(PlayerColor.Yellow);
        changeColor(PlayerColor.Yellow);
    }
    public void changeColor(PlayerColor c)
    {
        playerData.playerColor = playerColor = c;
        _dropdownImage.color = getColor(c);
        switch (c)
        {
            case PlayerColor.Red:
                icon.sprite = playerRedUI;
                break;
            case PlayerColor.Green:
                icon.sprite = playerGreenUI;
                break;
            case PlayerColor.Blue:
                icon.sprite = playerBlueUI;
                break;
            case PlayerColor.Yellow:
                icon.sprite = playerYellowUI;
                break;
        }
        playerData.Save(((int)playerIndex).ToString());
    }
    private Color getColor(PlayerColor c)
    {
        switch (c)
        {
            case PlayerColor.Red:
                return Color.red;
            case PlayerColor.Green:
                return Color.green;
            case PlayerColor.Blue:
                return Color.blue;
            case PlayerColor.Yellow:
                return Color.yellow;
        }
        return Color.black;
    }
    private void updateFinalColor(PlayerColor newcolor)
    {
        var functions = FindObjectsOfType<UI_PlayerManager>();
        if (functions != null)
        {
            for (int i = 0; i < functions.Length; i++)
            {
                if (functions[i].playerIndex == this.playerIndex) continue;
                if (functions[i].playerColor == newcolor)
                {
                    functions[i].changeColor(playerColor);
                }
            }
        }
    }
    #endregion
    #region Add Remove
    public void RemovePlayer()
    {
        if (AddPlayerButton != null) AddPlayerButton.gameObject.SetActive(true);
        playerData.enable = false;
        this.gameObject.SetActive(false);
        playerData.Save(((int)playerIndex).ToString());
    }
    public void AddPlayer()
    {
        if (AddPlayerButton != null) AddPlayerButton.gameObject.SetActive(false);
        playerData.enable = true;
        this.gameObject.SetActive(true);
        playerData.Save(((int)playerIndex).ToString());
    }
    #endregion
    #endregion
}
