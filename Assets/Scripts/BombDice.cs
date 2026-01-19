using System.Collections;
using UnityEngine;

public class BombDice : MonoBehaviour
{
    public Camera targetCamera;
    public Vector3 screenOffset = new Vector3(-0.4f, -0.2f, 1.2f);

    public float rollDuration = 0.8f;
    public float disappearDelay = 0.6f;
    public float rotationPower = 900f;

    private bool isRolling = false;

    void Awake()
    {
        gameObject.SetActive(false);
    }

    void LateUpdate()
    {
        if (!gameObject.activeSelf || targetCamera == null) return;

        Transform cam = targetCamera.transform;
        transform.position =
            cam.position +
            cam.right * screenOffset.x +
            cam.up * screenOffset.y +
            cam.forward * screenOffset.z;

        transform.LookAt(cam);
    }

    public IEnumerator Roll(System.Action<int> onResult)
    {
        if (isRolling) yield break;

        isRolling = true;
        gameObject.SetActive(true);

        float t = 0f;
        while (t < rollDuration)
        {
            transform.Rotate(
                rotationPower * Time.deltaTime,
                rotationPower * 0.8f * Time.deltaTime,
                rotationPower * 1.2f * Time.deltaTime,
                Space.World
            );

            t += Time.deltaTime;
            yield return null;
        }

        int result = Random.Range(1, 5); // d4
        SetFinalRotation(result);

        onResult?.Invoke(result);

        yield return new WaitForSeconds(disappearDelay);

        gameObject.SetActive(false);
        isRolling = false;
    }

    void SetFinalRotation(int value)
    {
        // ориентации под твой d4 — возможно придётся подкрутить
        switch (value)
        {
            case 1: transform.localRotation = Quaternion.Euler(0, 0, 0); break;
            case 2: transform.localRotation = Quaternion.Euler(0, 120, 0); break;
            case 3: transform.localRotation = Quaternion.Euler(0, 240, 0); break;
            case 4: transform.localRotation = Quaternion.Euler(180, 0, 0); break;
        }

        transform.LookAt(targetCamera.transform);
    }
}
