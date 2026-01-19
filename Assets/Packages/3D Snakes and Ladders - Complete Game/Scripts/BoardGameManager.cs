using MyDice;
using UnityEngine;
using UnityEngine.UI;
using MyDice.Board;
using MyDice.Players;
using System.Collections.Generic;

public class BoardGameManager : ElementNodeCreator
{
    [Header("UI")]
    public GameObject DiceRollingButton;
    [Header("UI - Dices")]
    public GameObject Dice4;
    public GameObject Dice6;
    public GameObject Dice8;
    public GameObject Dice10;
    [Header("UI - Ranking")]
    public GameObject finishDialogue;
    public Image FirstWinner;
    public Image SecondWinner;
    public Image ThirdWinner;

    [Header("Player Sprite(s)")]
    public Image playerTurn;
    [Header("Red")]
    public Sprite playerRedUI;
    public GameObject playerRedPrefab;
    [Header("Green")]
    public Sprite playerGreenUI;
    public GameObject playerGreenPrefab;
    [Header("Blue")]
    public Sprite playerBlueUI;
    public GameObject playerBluePrefab;
    [Header("Yellow")]
    public Sprite playerYellowUI;
    public GameObject playerYellowPrefab;

    #region private
    [HideInInspector] public bool btnHited;
    public bool GameIsOver { get { return gameIsOver; } }
    [HideInInspector] public bool gameIsOver = false;
    [HideInInspector] public int totalPlayers;
    [HideInInspector] public int totalRolling;
    [HideInInspector] public List<int> finishedPlayers;
    private GameSetting gameSetting;
    private bool rankingInited = false;
    #endregion
    private void Start()
    {
        gameSetting = new GameSetting();
        gameSetting.Load();

        initDice(gameSetting.diceMode);
        {
            Button btn;
            if (DiceRollingButton != null && (btn = DiceRollingButton.GetComponent<Button>()) != null)
            {
                btn.onClick.AddListener(onBtnDiceHit);
            }
        }
        diceManager = FindObjectOfType<DiceManager>();
        if (diceManager == null)
        {
            Debug.LogError("Dice manager not found.");
            Extensions.Quit();
        }
        initPlayers_fromSetting(gameSetting.players);
        base.initPlayers();
        finishedPlayers = new List<int>();
        gameIsOver = btnHited = false;
        for (int i = 0; i < gameSetting.players.Length; i++)
            if (gameSetting.players[i].enable) totalPlayers++;
        playerTurnManager(playerHomeIndex);
    }
    private void FixedUpdate()
    {
        if (diceManager.getDiceState() == DiceState.Ready)
        {
            if (gameIsOver)
            {
                showRankingWindow();
                return;
            }
            rolling();
        }
        else if (diceManager.getDiceState() == DiceState.Finish)
        {
            switch (playerHomes[playerHomeIndex].playerMode)
            {
                case PlayerMode.Human:
                    Player player = playerHomes[playerHomeIndex].getCandidatePlayer();
                    if (player == null)
                    {
                        checkForCandidate_Human(playerHomeIndex);
                        return;
                    }
                    updatePlayerGame_Human(player);
                    break;
                case PlayerMode.CPU:
                    updatePlayerGame_CPU();
                    break;
            }

            Player node = playerHomes[playerHomeIndex].getCandidatePlayer();
            if (node != null)
            {
                if (node.currentPositionIndex == playerHomes[playerHomeIndex].targetIndex)
                {
                    if (finishedPlayers.IndexOf(playerHomeIndex) < 0)
                    {
                        playerHomes[playerHomeIndex].enable = false;
                        finishedPlayers.Add(playerHomeIndex);
                        checkGameIsOver();
                        if (gameIsOver)
                        {
                            for (int i = 0; i < playerHomes.Count; i++)
                            {
                                if (playerHomes[i].enable && finishedPlayers.IndexOf(i) < 0)
                                {
                                    finishedPlayers.Add(i);
                                }
                            }
                        }
                    }
                }
            }
        }
        DiceRollingButton.SetActive(allowRolling);
    }

    private void showRankingWindow()
    {
        if (rankingInited) return;
        FirstWinner.gameObject.SetActive(false);
        SecondWinner.gameObject.SetActive(false);
        ThirdWinner.gameObject.SetActive(false);
        rankingInited = true;
        if (finishedPlayers.Count > 0)
        {
            FirstWinner.gameObject.SetActive(true);
            FirstWinner.sprite = getPlayerSprite(gameSetting.players[finishedPlayers[0]].playerColor);
        }
        if (finishedPlayers.Count > 1)
        {
            SecondWinner.gameObject.SetActive(true);
            SecondWinner.sprite = getPlayerSprite(gameSetting.players[finishedPlayers[1]].playerColor);
        }
        if (finishedPlayers.Count > 2)
        {
            ThirdWinner.gameObject.SetActive(true);
            ThirdWinner.sprite = getPlayerSprite(gameSetting.players[finishedPlayers[2]].playerColor);
        }
        finishDialogue.SetActive(true);
    }
    protected override void rolling()
    {
        if (!allowRolling) return;
        switch (playerHomes[playerHomeIndex].playerMode)
        {
            case PlayerMode.Human:
                if (btnHited)
                {
                    btnHited = false;
                    rollingFunction();
                }
                break;
            case PlayerMode.CPU:
                rollingFunction();
                break;
        }
    }
    private void rollingFunction()
    {
        diceManager.setFlag(true);
        allowRolling = false;
        totalRolling++;
    }
    public void onBtnDiceHit()
    {
        btnHited = true;
    }
    protected override void nextPlayer()
    {
        if (gameIsOver) return;
        playerHomeIndex = (playerHomeIndex + 1) % playerHomes.Count;
        if (playerHomes[playerHomeIndex].enable)
            playerTurnManager(playerHomeIndex);
        else
            nextPlayer();
    }
    protected bool checkGameIsOver()
    {
        if (totalPlayers == 1)
        {
            return gameIsOver = (finishedPlayers.Count == 1);
        }
        return gameIsOver = (finishedPlayers.Count == totalPlayers - 1);
    }
    private void playerTurnManager(int index)
    {
        if (playerTurn == null) return;
        playerTurn.sprite = getPlayerSprite(gameSetting.players[index].playerColor);
    }
    private Sprite getPlayerSprite(PlayerColor color)
    {
        switch (color)
        {
            case PlayerColor.Blue:
                return playerBlueUI;
            case PlayerColor.Green:
                return playerGreenUI;
            case PlayerColor.Red:
                return playerRedUI;
            case PlayerColor.Yellow:
                return playerYellowUI;
        }
        return playerYellowUI;
    }
    private void initDice(DiceMode diceMode)
    {
        switch (diceMode)
        {
            case DiceMode.Dice4:
                Dice4.SetActive(true);
                Object.DestroyImmediate(Dice6);
                Object.DestroyImmediate(Dice8);
                Object.DestroyImmediate(Dice10);
                break;
            case DiceMode.Dice6:
                Dice6.SetActive(true);
                Object.DestroyImmediate(Dice4);
                Object.DestroyImmediate(Dice8);
                Object.DestroyImmediate(Dice10);
                break;
            case DiceMode.Dice8:
                Dice8.SetActive(true);
                Object.DestroyImmediate(Dice4);
                Object.DestroyImmediate(Dice6);
                Object.DestroyImmediate(Dice10);
                break;
            case DiceMode.Dice10:
                Dice10.SetActive(true);
                Object.DestroyImmediate(Dice4);
                Object.DestroyImmediate(Dice6);
                Object.DestroyImmediate(Dice8);
                break;
        }
    }
    private void initPlayers_fromSetting(PlayerData[] players)
    {
        for (int i = 0; i < players.Length; i++)
        {
            playerHomes[i].enable = players[i].enable;
            if (players[i].enable)
            {
                playerHomes[i].playerMode = players[i].playerMode;
                playerHomes[i].updatePrefab(getPrefabFromColor(players[i].playerColor));
            }
            else
            {
                playerHomes[i].onDestroy();
            }
        }
    }
    private GameObject getPrefabFromColor(PlayerColor color)
    {
        switch (color)
        {
            case PlayerColor.Red:
                return playerRedPrefab;
            case PlayerColor.Green:
                return playerGreenPrefab;
            case PlayerColor.Blue:
                return playerBluePrefab;
        }
        return playerYellowPrefab;
    }
}
