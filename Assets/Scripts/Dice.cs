using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dice : MonoBehaviour
{

    [Header("Visual")]
    public Renderer diceRenderer;
    public Color normalColor = Color.white;
    public Color bombColor = Color.red;

    [Header("UI")]
    public Button rollButton;

    [Header("Dice Settings")]
    public float rollDuration = 1f;
    public float spinSpeed = 720f;

    [Header("Camera Follow")]
    public Camera targetCamera;
    public Vector3 cameraOffset = new Vector3(0.6f, -0.4f, 2.0f);
    public float followSmooth = 10f;

    [Header("References")]
    public GameManager gameManager;

    public int lastRoll { get; private set; } = 1;

    private bool isRolling = false;

    private Dictionary<int, Quaternion> faceToForward;

    void Awake()
    {
        // ориентации граней стандартного Unity Cube
        faceToForward = new Dictionary<int, Quaternion>
        {
            { 1, Quaternion.Euler(0,   0,   0) },
            { 2, Quaternion.Euler(0, 180,   0) },
            { 3, Quaternion.Euler(0, -90,   0) },
            { 4, Quaternion.Euler(0,  90,   0) },
            { 5, Quaternion.Euler(-90, 0,   0) },
            { 6, Quaternion.Euler(90,  0,   0) }
        };
    }

    void Start()
    {
        if (rollButton != null)
            rollButton.onClick.AddListener(OnRollPressed);
    }

    public void SetBombVisual(bool active)
    {
        if (!diceRenderer) return;

        diceRenderer.material.color = active ? bombColor : normalColor;
    }

    void LateUpdate()
    {
        FollowCamera();
    }

    void FollowCamera()
    {
        if (targetCamera == null) return;

        Vector3 desiredPos =
            targetCamera.transform.TransformPoint(cameraOffset);

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPos,
            Time.deltaTime * followSmooth
        );
    }

    void OnRollPressed()
    {
        if (!isRolling)
            StartCoroutine(RollDice());
    }

    IEnumerator RollDice()
    {
        isRolling = true;
        rollButton.interactable = false;

        float t = 0f;

        while (t < rollDuration)
        {
            transform.Rotate(
                spinSpeed * Time.deltaTime * Random.Range(1, 7),
                spinSpeed * 0.8f * Time.deltaTime * Random.Range(1, 7),
                spinSpeed * 1.2f * Time.deltaTime * Random.Range(1, 7),
                Space.World
            );

            t += Time.deltaTime;
            yield return null;
        }


        lastRoll = Random.Range(1, 7);
        ApplyFinalRotation(lastRoll);

        Debug.Log("Dice rolled: " + lastRoll);

        gameManager.OnDiceRolled(lastRoll);

        isRolling = false;
    }

    public void ShowValue(int value)
    {
        value = Mathf.Clamp(value, 1, 6);
        SetFinalRotation(value);
    }


    void SetFinalRotation(int value)
    {
        value = Mathf.Clamp(value, 1, 6);

        Quaternion faceRotation;

        // Какая грань "смотрит вперед" у модели
        switch (value)
        {
            case 1: faceRotation = Quaternion.Euler(0, 0, 0); break;
            case 2: faceRotation = Quaternion.Euler(0, 180, 0); break;
            case 3: faceRotation = Quaternion.Euler(0, -90, 0); break;
            case 4: faceRotation = Quaternion.Euler(0, 90, 0); break;
            case 5: faceRotation = Quaternion.Euler(-90, 0, 0); break;
            case 6: faceRotation = Quaternion.Euler(90, 0, 0); break;
            default: faceRotation = Quaternion.identity; break;
        }

        if (targetCamera == null)
            return;

        // Смотрим в камеру
        Quaternion lookAtCamera =
            Quaternion.LookRotation(
                targetCamera.transform.position - transform.position,
                Vector3.up
            );

        // Итоговый поворот
        transform.rotation = lookAtCamera * faceRotation;
    }


    void ApplyFinalRotation(int value)
    {
        if (targetCamera == null) return;

        Vector3 toCamera =
            (targetCamera.transform.position - transform.position).normalized;

        Quaternion lookAtCamera =
            Quaternion.LookRotation(toCamera, Vector3.up);

        Quaternion faceRotation = faceToForward[value];

        // КЛЮЧЕВОЙ МОМЕНТ — порядок умножения
        transform.rotation = lookAtCamera * faceRotation;
    }

    public IEnumerator RollForBot()
    {
        if (isRolling)
            yield break;

        yield return StartCoroutine(RollDice());
    }

    public void EnableDiceButton(bool enable)
    {
        rollButton.interactable = enable;
    }
}
