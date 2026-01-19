using UnityEngine;
using System.Collections;

public class PlayerDataBase : MonoBehaviour
{
    public Transform pawn; //Фишка модельки
    public GameObject shieldVisual;
    public int currentTileIndex = 0;
    public bool isBot = false;

    public bool hasBombDebuff = false;
    public int bombPenalty = 0;

    public bool hasShield = false;

    [Header("Rotation Fix")]
    public Vector3 modelRotationOffset = new Vector3(0, 90, 0);

    public IEnumerator MoveBackward(
    int steps,
    BoardGenerator board,
    System.Action onFinish = null
)
    {
        for (int i = 0; i < steps; i++)
        {
            RingData ring = board.GetRingForTile(currentTileIndex);

            int prev = currentTileIndex - 1;
            if (prev < ring.startIndex)
                prev = ring.endIndex;

            currentTileIndex = prev;
            yield return MoveTo(board.tiles[currentTileIndex].position);
        }

        onFinish?.Invoke();
    }


    public IEnumerator MoveTo(Vector3 target)
    {
        Vector3 start = pawn.position;
        float t = 0f;

        Vector3 dir = target - start;
        dir.y = 0;

        if (dir.sqrMagnitude > 0.001f)
        {
            dir.Normalize();
        }

        Quaternion targetRot =
            Quaternion.LookRotation(dir, Vector3.up) *
            Quaternion.Euler(modelRotationOffset);

        while (t < 1f)
        {
            t += Time.deltaTime * 3f;

            pawn.position = Vector3.Lerp(start, target, t);

            pawn.rotation = Quaternion.Slerp(
                pawn.rotation,
                targetRot,
                Time.deltaTime * 8f
            );

            yield return null;
        }

        pawn.position = target;
        pawn.rotation = targetRot;
    }

    public void SetShield(bool active)
    {
        hasShield = active;
        if (shieldVisual != null)
            shieldVisual.SetActive(active);
    }
}
