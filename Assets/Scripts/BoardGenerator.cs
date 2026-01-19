using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public enum BonusType
{
    None,
    MinusOne,
    PlusOne,
    Bomb,
    Shield,
    CursedDice
}

public class BoardGenerator : MonoBehaviour
{
    [Header("Tile Prefab")]
    public GameObject tilePrefab;

    [Header("Board Parameters")]
    public int tilesCount = 100;
    public int rows = 10;                 // ← количество клеток в одном ряду
    public float tileHeightY = 0.2f;
    public float tileMargin = 0.9f;

    [Header("Snakes & Ladders")]
    public int jumpCount = 8;             // сколько переходов создаём

    [Header("Сircle board params")]
    // ========= ПАРАМЕТРЫ КРУГОВ =========
    public int ring = 0;                      // текущий круг
    private int tilesInRing = 0;
    public int baseTilesInRing = 10;               // стартовое число тайлов во внутреннем круге
    public int tileIncreasePerRing = 10;       // насколько увеличиваем тайлы каждого следующего круга
    public float baseRadius = 0.5f;           // радиус внутреннего круга
    public float radiusStep = 0.55f;          // расстояние между кругами
    public List<RingData> rings = new List<RingData>();


    [Header("Bonuses")]
    public int bonusTilesCount = 5;
    public Dictionary<int, BonusType> bonusMap = new Dictionary<int, BonusType>();
    public PlayerInventory inventory;

    [Header("Board")]
    public Renderer boardRenderer;


    public List<Transform> tiles = new List<Transform>();
    public Dictionary<int, int> jumpMap = new Dictionary<int, int>();



    public RingData GetRingForTile(int tileIndex)
    {
        foreach (var ring in rings)
        {
            if (tileIndex >= ring.startIndex && tileIndex <= ring.endIndex)
                return ring;
        }
        return null;
    }

    void GenerateRingLadders()
    {
        jumpMap.Clear();

        for (int i = 0; i < rings.Count - 1; i++)
        {
            int from = rings[i].endIndex;
            int to = rings[i + 1].startIndex;

            jumpMap[from] = to;

            if (from >= 0 && from < tiles.Count)
            {
                Transform tile = tiles[from];
                Renderer rend = tile.GetComponentInChildren<Renderer>();

                if (rend != null)
                {
                    // ВАЖНО: создаём новый материал
                    rend.material = new Material(rend.material);
                    rend.material.color = Color.green;
                }
            }
        }


    }

    public void GenerateCircleBoard()
    {
        ring = 0;
        tilesInRing = baseTilesInRing;
        rings.Clear();

        if (!boardRenderer)
        {
            Debug.LogError("BoardRenderer required!");
            return;
        }

        // Чистим старое
        for (int i = transform.childCount - 1; i >= 0; i--)
            DestroyImmediate(transform.GetChild(i).gameObject);

        tiles.Clear();
        jumpMap.Clear();
        bonusMap.Clear();

        

        Vector3 center = boardRenderer.bounds.center;
        int index = 0;

        // ========= СОЗДАЁМ КОЛЬЦА =========
        while (index < tilesCount)
        {
            int ringStartIndex = index;

            float radius = baseRadius + ring * radiusStep;

            for (int i = 0; i < tilesInRing && index < tilesCount; i++)
            {
                float angle = (float)i / tilesInRing * Mathf.PI * 2f;

                float x = center.x + Mathf.Cos(angle) * radius;
                float z = center.z + Mathf.Sin(angle) * radius;

                GameObject tile = Instantiate(tilePrefab, transform);
                tile.name = $"Tile_{index}";

                tile.transform.position = new Vector3(x, tileHeightY, z);
                tile.transform.rotation = Quaternion.Euler(0, -angle * Mathf.Rad2Deg + 90f, 0);

                float scale = radiusStep * 0.55f;
                tile.transform.localScale = new Vector3(scale, 0.1f, scale);

                tiles.Add(tile.transform);
                index++;
            }

            int ringEndIndex = index - 1;

            rings.Add(new RingData
            {
                startIndex = ringStartIndex,
                endIndex = ringEndIndex
            });

            ring++;
            tilesInRing += tileIncreasePerRing;
        }

        // Далее обычные функции
        GenerateRingLadders();

        GenerateBonuses();
        ApplyBonusIcons();
        ClearDecorationsByColliderIntersection();
    }


    
    void ApplyBonusIcons()
    {
        foreach (var kv in bonusMap)
        {
            int index = kv.Key;
            Debug.Log("index= " + index + " ");
            BonusType type = kv.Value;

            if (index < 0 || index >= tiles.Count) continue;

            var tile = tiles[index];
            var icon = tile.GetComponent<TileBonusIcon>();

            if (icon != null && inventory != null)
            {
                
                icon.SetIcon(inventory.GetBonusIcon(type));
            }
        }
    }

    

    void ClearDecorationsByColliderIntersection()
    {
        // Находим все декорации
        GameObject[] decorations = GameObject.FindGameObjectsWithTag("Decoration");

        HashSet<GameObject> toRemove = new HashSet<GameObject>();

        foreach (Transform tile in tiles)
        {
            Collider tileCol = tile.GetComponent<Collider>();
            if (!tileCol) continue;

            foreach (GameObject deco in decorations)
            {
                if (toRemove.Contains(deco)) continue;

                // Берём ВСЕ коллайдеры декорации (включая детей)
                Collider[] decoCols = deco.GetComponentsInChildren<Collider>();

                foreach (Collider decoCol in decoCols)
                {
                    if (!decoCol || decoCol.isTrigger) continue;

                    if (tileCol.bounds.Intersects(decoCol.bounds))
                    {
                        toRemove.Add(deco);
                        break;
                    }
                }
            }
        }

        foreach (var deco in toRemove)
        {
            Destroy(deco);
        }
    }
    // ----------------------
    // БОНУСЫ
    // ----------------------
    void GenerateBonuses()
    {
        HashSet<int> used = new HashSet<int>();

        while (bonusMap.Count < bonusTilesCount)
        {
            int idx = Random.Range(2, tilesCount - 2);
            if (used.Contains(idx) || jumpMap.ContainsKey(idx)) continue;

            used.Add(idx);
            BonusType type = (BonusType)Random.Range(1, 6);
            bonusMap[idx] = type;
        }

        Debug.Log("Bonus map: " + string.Join(", ", bonusMap));
    }

   
}

[System.Serializable]
public class RingData
{
    public int startIndex;
    public int endIndex;
}
