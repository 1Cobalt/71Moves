using UnityEngine;
using UnityEngine.UI;

public class TileBonusIcon : MonoBehaviour
{
    public Image icon;   // UI-иконка над тайлом

    public void SetIcon(Sprite sprite)
    {
        icon.sprite = sprite;
        icon.enabled = (sprite != null);
    }

    public void Clear()
    {
        icon.sprite = null;
        icon.enabled = false;
    }
}