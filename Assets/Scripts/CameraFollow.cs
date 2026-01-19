using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    [Header("Offsets")]
    public Vector3 offset = new Vector3(0, 6f, -6f);

    [Header("Follow")]
    public float followSpeed = 5f;

    [Header("Switch")]
    public float switchDuration = 0.6f;

    Transform followTarget;   // активная цель
    Transform pendingTarget;  // цель перелёта

    Coroutine switchRoutine;

    void LateUpdate()
    {
        if (switchRoutine != null || followTarget == null)
            return;

        Vector3 desiredPos = followTarget.position + offset;

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPos,
            followSpeed * Time.deltaTime
        );

        Quaternion rot = Quaternion.LookRotation(
            followTarget.position - transform.position,
            Vector3.up
        );

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            rot,
            followSpeed * Time.deltaTime
        );
    }

    public void SetTarget(Transform newTarget)
    {
        if (switchRoutine != null)
            StopCoroutine(switchRoutine);

        pendingTarget = newTarget;
        switchRoutine = StartCoroutine(SmoothSwitch());
    }

    IEnumerator SmoothSwitch()
    {
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        Vector3 endPos = pendingTarget.position + offset;
        Quaternion endRot = Quaternion.LookRotation(
            pendingTarget.position - endPos,
            Vector3.up
        );

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / switchDuration;

            float eased = Mathf.SmoothStep(0f, 1f, t);

            transform.position = Vector3.Lerp(startPos, endPos, eased);
            transform.rotation = Quaternion.Slerp(startRot, endRot, eased);

            yield return null;
        }

        followTarget = pendingTarget;
        pendingTarget = null;
        switchRoutine = null;
    }
}
