// --- TileJumpIndicator.cs ---
// Добавляет визуализацию змей/лестниц прямо на тайле во время игры.
// Показывает стрелку + подсветку направления.
using UnityEngine;


public class TileJumpIndicator : MonoBehaviour
{
    public int tileIndex; // автоматически определяется
    public GameManager manager;


    private GameObject arrowIcon;


    public int ringIndex = 0;
    public int tileIndexInRing = 0;


    private TMPro.TextMeshPro textLabel;


    void Start()
    {
        // Создаём объект иконки
        arrowIcon = new GameObject("JumpArrow");
        arrowIcon.transform.SetParent(transform);
        arrowIcon.transform.localPosition = new Vector3(0, 0.3f, 0);
        arrowIcon.transform.localScale = Vector3.one * 0.4f;

        manager = GameObject.Find("GameManager").GetComponent<GameManager>();

        

        var sr = arrowIcon.AddComponent<SpriteRenderer>();
        sr.sortingOrder = 10;


        arrowIcon.SetActive(false);


        // авто-поиск TMP потомка
        textLabel = GetComponentInChildren<TMPro.TextMeshPro>();
        if (textLabel != null)
            textLabel.text = this.name.Replace("Tile_", "");
    }

}