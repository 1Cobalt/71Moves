
using UnityEngine;
using System.Collections;

public class TileHighlighter : MonoBehaviour
{
    public Color flashColor = new Color(1f, 0.8f, 0.2f); // тёплый жёлтый
    public float flashDuration = 0.15f;

    private Renderer rend;
    private Color originalColor;
    private Material runtimeMaterial;
    private Coroutine flashRoutine;

    void Awake()
    {
        rend = GetComponentInChildren<Renderer>();
        if (!rend) return;

        // Создаём инстанс материала
        runtimeMaterial = new Material(rend.material);
        rend.material = runtimeMaterial;

        originalColor = runtimeMaterial.color;
    }

    public void Flash()
    {
        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

        flashRoutine = StartCoroutine(FlashRoutine());
    }

    IEnumerator FlashRoutine()
    {
        runtimeMaterial.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        runtimeMaterial.color = originalColor;
    }
}
