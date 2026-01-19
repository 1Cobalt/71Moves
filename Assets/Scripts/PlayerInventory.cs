using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    public Image[] itemSlots;   // UI слоты
    public Sprite minusOneSprite;
    public Sprite plusOneSprite;
    public Sprite bombSprite;
    public Sprite shieldSprite;
    public Sprite cursedDiceSprite;


    public BonusType[] items = new BonusType[3]; // храним предметы через enum, без null

    // --------------------------------------------------
    // ДОБАВИТЬ ПРЕДМЕТ
    // --------------------------------------------------
    public bool AddItem(BonusType item, PlayerDataBase owner)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == BonusType.None)
            {
                items[i] = item;
                UpdateSlotUI(i, item);
                return true;
            }
        }
        return false;
    }

    public void SetVisible(bool visible)
    {
        gameObject.SetActive(visible);
    }


    // --------------------------------------------------
    // ИСПОЛЬЗОВАТЬ ПРЕДМЕТ
    // --------------------------------------------------
    public BonusType? UseItem(int index)
    {
        if (index < 0 || index >= items.Length)
            return null;

        if (items[index] == BonusType.None)
            return null;

        BonusType used = items[index];
        items[index] = BonusType.None;

        ClearSlotUI(index);

        return used;
    }

    // --------------------------------------------------
    // ПРОВЕРКА: ЕСТЬ ЛИ ХОТЬ ОДИН ПРЕДМЕТ
    // --------------------------------------------------
    public bool HasUsableItems()
    {
        for (int i = 0; i < items.Length; i++)
            if (items[i] != BonusType.None)
                return true;

        return false;
    }

    // --------------------------------------------------
    // UI: отрисовать всю инвенторию (при старте матча)
    // --------------------------------------------------
    public void RefreshUI()
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == BonusType.None)
                ClearSlotUI(i);
            else
                UpdateSlotUI(i, items[i]);
        }
    }

    // --------------------------------------------------
    // UI: поставить иконку предмета
    // --------------------------------------------------
    private void UpdateSlotUI(int index, BonusType item)
    {
        var icon = GetBonusIcon(item);
        itemSlots[index].sprite = icon;
        itemSlots[index].color = Color.white;
        itemSlots[index].raycastTarget = true;
    }

    // --------------------------------------------------
    // UI: очистить слот
    // --------------------------------------------------
    private void ClearSlotUI(int index)
    {
        itemSlots[index].sprite = null;
        itemSlots[index].color = Color.clear;
        itemSlots[index].raycastTarget = false;
    }

    // --------------------------------------------------
    // Получить иконку для бонуса
    // --------------------------------------------------
    public Sprite GetBonusIcon(BonusType type)
    {
        switch (type)
        {
            case BonusType.MinusOne: return minusOneSprite;
            case BonusType.PlusOne: return plusOneSprite;
            case BonusType.Bomb: return bombSprite;
            case BonusType.Shield: return shieldSprite;
            case BonusType.CursedDice: return cursedDiceSprite;
        }
        return null;
    }
}
