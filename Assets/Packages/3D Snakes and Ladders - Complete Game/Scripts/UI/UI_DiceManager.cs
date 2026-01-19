using UnityEngine;
using UnityEngine.UI;

public class UI_DiceManager : MonoBehaviour
{
    #region variable
    public Image DiceSelectImage;
    public DiceMode dice;

    [Header("Dice images")]
    public Sprite DiceIdle;
    public Sprite DicePress;
    #endregion
    #region Functions
    private void Start()
    {
        GameSetting gameSetting = new GameSetting();
        gameSetting.Load();
        if (gameSetting.diceMode == dice)
        {
            Dice_select();
        }
    }
    #endregion
    #region functions
    #region select dice
    public void Dice_select()
    {
        updateDiceSelectImage(dice, DiceIdle, DicePress);
    }
    protected void updateDiceSelectImage(DiceMode diceMode, Sprite idle, Sprite press)
    {
        GameSetting gameSetting = new GameSetting();
        gameSetting.Load();
        gameSetting.diceMode = diceMode;
        gameSetting.Save();

        if (idle != null)
        {
            Image img = DiceSelectImage.GetComponent<Image>();
            if (img != null)
            {
                img.sprite = idle;
            }
        }
        if (press != null)
        {
            Button btn = DiceSelectImage.GetComponent<Button>();
            if (btn != null)
            {
                var sprState = btn.spriteState;
                sprState.pressedSprite = press;
                btn.spriteState = sprState;
            }
        }
    }

    #endregion
    #endregion
}
