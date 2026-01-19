using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WinManager : MonoBehaviour
{
    public static WinManager Instance;

    public GameObject winScreen;      // Панель победы / окно
    public GameObject inGameHud;
    public TextMeshProUGUI winText;              // Текст "Победил игрок 1"

    private void Awake()
    {
        Instance = this;

        if (winScreen != null)
            winScreen.SetActive(false);
    }

    public void DeclareWinner(int playerIndex)
    {
        // Блокируем игровой процесс
        GameObject.Find("GameManager").GetComponent<GameManager>().gameOver = true;

        // Показываем экран победы
        winScreen.SetActive(true);
        inGameHud.SetActive(false);
        winText.text = $"Player {playerIndex} wins!";
    }
}