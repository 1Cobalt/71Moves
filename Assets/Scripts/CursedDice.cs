using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursedDice : MonoBehaviour
{
    [Header("Positioning")]
    public Camera targetCamera;
    public Vector3 screenOffset = new Vector3(0.4f, -0.2f, 1.2f);

    [Header("Roll Settings")]
    public float rollDuration = 1.0f;
    public float disappearDelay = 1.0f;
    public float rotationPower = 900f;

    private bool isRolling = false;

    // Локальные повороты граней
    // ВАЖНО: меш куба должен смотреть +Z вперёд
    private Dictionary<int, Quaternion> faceToForward;

    void Awake()
    {
        gameObject.SetActive(false);

        faceToForward = new Dictionary<int, Quaternion>
        {
            { 1, Quaternion.identity },           // Z+
            { 2, Quaternion.Euler(0, 180, 0) },    // Z-
            { 3, Quaternion.Euler(0, -90, 0) },    // X+
            { 4, Quaternion.Euler(0, 90, 0) },     // X-
            { 5, Quaternion.Euler(-90, 0, 0) },    // Y+
            { 6, Quaternion.Euler(90, 0, 0) }      // Y-
        };
    }

    void LateUpdate()
    {
        if (!gameObject.activeSelf || targetCamera == null)
            return;

        Transform cam = targetCamera.transform;

        // --- позиция перед камерой ---
        transform.position =
            cam.position +
            cam.right * screenOffset.x +
            cam.up * screenOffset.y +
            cam.forward * screenOffset.z;

        // --- пока не крутимся — просто смотрим в камеру ---
        if (!isRolling)
        {
            transform.rotation = Quaternion.LookRotation(
                -cam.forward,
                cam.up
            );
        }
    }

    // --------------------------------------------------
    // ПУБЛИЧНЫЙ ВЫЗОВ
    // --------------------------------------------------
    public IEnumerator Roll(System.Action<int> onResult)
    {
        if (isRolling)
            yield break;

        isRolling = true;
        gameObject.SetActive(true);

        float t = 0f;

        // --- фаза кручения ---
        while (t < rollDuration)
        {
            transform.Rotate(
                rotationPower * Time.deltaTime,
                rotationPower * 0.8f * Time.deltaTime,
                rotationPower * 1.2f * Time.deltaTime,
                Space.Self // ❗ критично
            );

            t += Time.deltaTime;
            yield return null;
        }

        // --- результат ---
        int result = Random.Range(1, 7);
        onResult?.Invoke(result);

        // --- показать нужную грань ---
        SetFinalRotation(result);

        yield return new WaitForSeconds(disappearDelay);

        gameObject.SetActive(false);
        isRolling = false;
    }

    // --------------------------------------------------
    // ФИНАЛЬНАЯ ОРИЕНТАЦИЯ
    // --------------------------------------------------
    private void SetFinalRotation(int value)
    {
        if (targetCamera == null)
            return;

        Transform cam = targetCamera.transform;

        // 1️⃣ Базовая ориентация: куб смотрит в камеру
        Quaternion cameraFacing = Quaternion.LookRotation(
            -cam.forward,
            cam.up
        );

        // 2️⃣ Локальный поворот нужной грани
        Quaternion faceRotation = faceToForward[value];

        // 3️⃣ Итог
        transform.rotation = cameraFacing * faceRotation;
    }
}
