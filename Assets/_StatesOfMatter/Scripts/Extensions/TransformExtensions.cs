using UnityEngine;
using DG.Tweening;

public static class TransformExtensions 
{
    /// <summary>
    /// Tweens this transform into its parent's layout slot (or to a custom endPosition if given).
    /// </summary>
    public static void MoveToLocalPosition(this Transform transform, float moveTime, Vector3? endPosition = null)
    {
        // If endPosition not provided, default to the "slot position" (Vector3.zero relative to parent)
        Vector3 target = endPosition ?? Vector3.zero;

        // Kill any existing tweens on this transform to avoid conflicts
        transform.DOKill();

        // Animate to the correct slot
        transform.DOLocalMove(target, moveTime);
    }
}
