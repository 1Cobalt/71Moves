using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public enum GameMode
{
    PvP,
    PvBot
}

public class GameManager : MonoBehaviour
{
    [Header("Camera")]
    public CameraFollow cameraFollow;

    [Header("Links")]
    public BoardGenerator board;
    public PlayerInventory inventory1;
    public PlayerInventory inventory2;
    public PlayerDataBase player1;
    public PlayerDataBase player2;
    public Dice dice;
    public CursedDice cursedDice;
    public BombDice bombDice;
    public Dice mainDice;



    [Header("UI")]
    public Button acceptRollButton;
    public Button[] itemButtons;

    [Header("Runtime")]
    public int lastDiceRoll = 0;
    bool awaitingItemDecision = false;
    public bool gameOver = false;

    public GameMode gameMode = GameMode.PvP;

    private PlayerDataBase currentPlayer;
    private PlayerInventory currentInventory;

    public List<Transform> tiles => board.tiles;
    public Dictionary<int, int> jumpMap => board.jumpMap;


  

    private void Start()
    {
        board.GenerateCircleBoard();

    
        // Слушатели кнопок предметов
        for (int i = 0; i < itemButtons.Length; i++)
        {
            int slot = i;
            itemButtons[i].onClick.AddListener(() => TryUseItem(slot));
        }

        acceptRollButton.onClick.AddListener(OnAcceptRoll);
        gameOver = false;
       
        InitPlayers();
        StartTurn();
    }

    void InitPlayers()
    {
        player1.currentTileIndex = 0;
        player1.pawn.position = board.tiles[0].position;
        player1.isBot = false;

        player2.currentTileIndex = 0;
        player2.pawn.position = board.tiles[0].position;

        if (gameMode == GameMode.PvP)
            player2.isBot = false;
        else
            player2.isBot = true;

        currentPlayer = player1;
        currentInventory = inventory1;
    }




    void StartTurn()
    {
        inventory1.SetVisible(currentPlayer == player1);
        inventory2.SetVisible(currentPlayer == player2);

        dice.SetBombVisual(currentPlayer.hasBombDebuff);

        if (cameraFollow != null)
            cameraFollow.SetTarget(currentPlayer.pawn);

        if (currentPlayer.isBot)
        {
            StartCoroutine(BotTurn());
        }
        else
        {
            dice.EnableDiceButton(true);
            acceptRollButton.gameObject.SetActive(false);
            ShowItemSelectionUI(false);
        }
    }

    IEnumerator UseCursedDice(PlayerDataBase user)
    {
        dice.EnableDiceButton(false);
        ShowItemSelectionUI(false);

        PlayerDataBase enemy =
            user == player1 ? player2 : player1;

        cursedDice.gameObject.SetActive(true);

        // 🛡 Щит
        if (enemy.hasShield)
        {
            enemy.SetShield(false);
            Debug.Log("🛡 Shield blocked Cursed Dice!");
            yield break;
        }

        int rollResult = 0;

        yield return StartCoroutine(
            cursedDice.Roll(result => rollResult = result)
        );

        yield return new WaitForSeconds(0.2f);

        yield return StartCoroutine(
            enemy.MoveBackward(rollResult, board)
        );

        dice.EnableDiceButton(true);
    }



    private IEnumerator BotTurn()
    {
        dice.EnableDiceButton(false);

        yield return new WaitForSeconds(0.5f);

        // визуальный бросок кубика
        yield return StartCoroutine(dice.RollForBot());

        // после OnDiceRolled lastDiceRoll уже установлен
        if (currentInventory.HasUsableItems() && Random.value < 0.6f)
        {
            TryUseItemBot();
            yield return new WaitForSeconds(0.5f);
        }

        yield return MovePlayer(currentPlayer, lastDiceRoll);
    }


    void TryUseItemBot()
    {
        for (int i = 0; i < 3; i++)
        {
            var item = currentInventory.items[i];
            if (item == BonusType.None) continue;

            // 🎲 Cursed Dice — приоритет
            if (item == BonusType.CursedDice && Random.value < 0.2f)
            {
                currentInventory.UseItem(i);
                StartCoroutine(UseCursedDice(currentPlayer));
                return;
            }

            if (item == BonusType.PlusOne && lastDiceRoll < 6)
            {
                lastDiceRoll++;
                currentInventory.UseItem(i);
                dice.ShowValue(lastDiceRoll);
                return;
            }

            if (item == BonusType.MinusOne && lastDiceRoll > 1)
            {
                lastDiceRoll--;
                currentInventory.UseItem(i);
                dice.ShowValue(lastDiceRoll);
                return;
            }

            if (item == BonusType.Shield)
            {
                ActivateShield();
                currentInventory.UseItem(i);
                return;
            }

            if (item == BonusType.Bomb)
            {
                ApplyBombToOpponent();
                currentInventory.UseItem(i);
                return;
            }
        }
    }


    // -------------------------------------------------
    // КУБИК бросился → GameManager получает результат
    // -------------------------------------------------
    public void OnDiceRolled(int value)
    {
        lastDiceRoll = ApplyBombIfNeeded(currentPlayer, value);

        awaitingItemDecision = true;

        if (!currentPlayer.isBot)
        {
            ShowItemSelectionUI(true);
            dice.EnableDiceButton(false);
        }
    }

    void ActivateShield()
    {
        if (currentPlayer.hasShield)
        {
            Debug.Log("Shield already active");
            return;
        }

        currentPlayer.SetShield(true);

        Debug.Log("🛡 Shield activated!");
    }


    void ShowItemSelectionUI(bool enable)
    {
        acceptRollButton.gameObject.SetActive(enable);
        foreach (var btn in itemButtons)
            btn.gameObject.SetActive(enable);
    }

    // -------------------------------------------------
    // Использование предмета человеком
    // -------------------------------------------------
    public void TryUseItem(int slot)
    {
        var item = currentInventory.UseItem(slot);
        if (item == null) return;

        switch (item)
        {
            case BonusType.PlusOne:
                {
                    lastDiceRoll = Mathf.Min(6, lastDiceRoll + 1);
                    dice.ShowValue(lastDiceRoll); // 👈 визуально довернули
                    break;
                }

            case BonusType.MinusOne:
                {
                    lastDiceRoll = Mathf.Max(1, lastDiceRoll - 1);
                    dice.ShowValue(lastDiceRoll); // 👈 визуально довернули
                    break;
                }
            case BonusType.Bomb:
                ApplyBombToOpponent();
                break;

            case BonusType.Shield:
                ActivateShield();
                break;


            case BonusType.CursedDice:
                StartCoroutine(UseCursedDice(currentPlayer));
                break;


        }

        Debug.Log("New dice after item: " + lastDiceRoll);
    }

    IEnumerator UseCursedDice()
    {
        dice.EnableDiceButton(false);
        ShowItemSelectionUI(false);
        cursedDice.gameObject.SetActive(true);

        PlayerDataBase enemy =
            currentPlayer == player1 ? player2 : player1;

        if (enemy.hasShield)
        {
            enemy.SetShield(false);
            Debug.Log("🛡 Shield blocked Cursed Dice!");
            yield break;
        }

        int rollResult = 0;

        yield return StartCoroutine(
            cursedDice.Roll(result => rollResult = result)
        );

        yield return new WaitForSeconds(0.2f);

        yield return StartCoroutine(
            enemy.MoveBackward(rollResult, board)
        );

        dice.EnableDiceButton(true);
    }




    void ApplyBombToOpponent()
    {
        PlayerDataBase opponent =
            (currentPlayer == player1) ? player2 : player1;

        opponent.hasBombDebuff = true;

        Debug.Log("💣 Bomb applied to opponent!");
    }
    // -------------------------------------------------
    // Игрок подтверждает бросок
    // -------------------------------------------------
    void OnAcceptRoll()
    {
        awaitingItemDecision = false;
        ShowItemSelectionUI(false);

        StartCoroutine(MovePlayer(currentPlayer, lastDiceRoll));
        acceptRollButton.gameObject.SetActive(false);
    }

    // ------------------------------
    // ДВИЖЕНИЕ И ПОСЛЕДСТВИЯ
    // ------------------------------
    IEnumerator MovePlayer(PlayerDataBase p, int steps)
    {
        RingData ring = board.GetRingForTile(p.currentTileIndex);

        for (int i = 0; i < steps; i++)
        {
            int next = p.currentTileIndex + 1;

            // зацикливание внутри кольца
            if (next > ring.endIndex)
                next = ring.startIndex;

            p.currentTileIndex = next;

            // движение
            yield return p.MoveTo(tiles[p.currentTileIndex].position);

            // подсветка шага
            var highlight = tiles[p.currentTileIndex].GetComponent<TileHighlighter>();
            if (highlight != null)
                highlight.Flash();

            yield return new WaitForSeconds(0.15f);
        }

        // ⬆ ЛЕСТНИЦА / ЗМЕЯ — ТОЛЬКО ЕСЛИ ОСТАНОВИЛСЯ
        if (jumpMap.TryGetValue(p.currentTileIndex, out int dst))
        {
            p.currentTileIndex = dst;
            yield return p.MoveTo(tiles[dst].position);
        }

        // 🎁 БОНУС — ТОЛЬКО ФИНАЛЬНЫЙ ТАЙЛ
        CheckBonusOnTile(p, p.currentTileIndex);

        // 🏁 победа
        if (p.currentTileIndex == tiles.Count - 1)
        {
            WinManager.Instance.DeclareWinner(p == player1 ? 1 : 2);
            yield break;
        }

        EndTurn();
    }



    void CheckBonusOnTile(PlayerDataBase p, int index)
    {
        if (!board.bonusMap.TryGetValue(index, out BonusType bonus))
            return;

        // 1. Выдать бонус игроку
        if (p == player1) inventory1.AddItem(bonus, player1);
        if (p == player2) inventory2.AddItem(bonus, player2);

        Debug.Log($"Player picked bonus: {bonus}");

        // 2. УБРАТЬ БОНУС МЕХАНИЧЕСКИ
        board.bonusMap.Remove(index);

        // 3. УБРАТЬ БОНУС ВИЗУАЛЬНО
        Transform tile = board.tiles[index];
        TileBonusIcon icon = tile.GetComponent<TileBonusIcon>();
        if (icon != null)
        {
            icon.Clear();
        }
    }

    int ApplyBombIfNeeded(PlayerDataBase player, int roll)
    {
        if (!player.hasBombDebuff)
            return roll;

        if (player.hasShield)
        {
            player.SetShield(false);
            player.hasBombDebuff = false;
            dice.SetBombVisual(false);

            Debug.Log("🛡 Shield absorbed the bomb!");
            return roll;
        }

        int penalty = Random.Range(1, 5);
        int finalRoll = Mathf.Max(1, roll - penalty);

        player.hasBombDebuff = false;
        dice.SetBombVisual(false);

        Debug.Log($"💣 Bomb reduced roll by {penalty}");

        return finalRoll;
    }


    // ------------------------------
    // Смена хода
    // ------------------------------
    void EndTurn()
    {
        currentPlayer = (currentPlayer == player1) ? player2 : player1;
        currentInventory = (currentInventory == inventory1) ? inventory2 : inventory1;

        StartTurn();
    }


}
