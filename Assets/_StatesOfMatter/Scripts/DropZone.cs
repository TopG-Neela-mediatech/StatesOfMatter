using UnityEngine;
using UnityEngine.UI;

public enum Category { Solid, Liquid, Gas }

public class DropZone : MonoBehaviour
{
    public Category zoneCategory;

    [Header("Placement Mode")]
    [Tooltip("If true, will use explicit slots as children of the grid.")]
    [SerializeField] private bool useSlots = false;

    [Tooltip("Explicit slots inside this zone (only used if UseSlots = true)")]
    [SerializeField] private RectTransform[] setPoints;

    private GridLayoutGroup _grid;

    private void Awake()
    {
        _grid = GetComponent<GridLayoutGroup>();
        if (_grid == null)
        {
            Debug.LogWarning("DropZone works best with a GridLayoutGroup on the same GameObject.");
        }
    }

    /// <summary>
    /// Places draggable either in the next free slot, or appends to the grid.
    /// Returns true if placed successfully.
    /// </summary>
    public bool PlaceDraggable(MatterDraggable draggable)
    {
        if (useSlots && setPoints != null && setPoints.Length > 0)
        {
            // SLOT-BASED MODE
            for (int i = 0; i < setPoints.Length; i++)
            {
                Transform slot = setPoints[i];
                if (slot.childCount == 0) // empty slot
                {
                    draggable.transform.SetParent(slot, false);
                    draggable.transform.MoveToLocalPosition(.5f);
                    return true;
                }
            }

            Debug.LogWarning($"No empty slot available in {zoneCategory} zone!");
            return false;
        }
        else
        {
            // AUTO-GRID MODE
            Vector3 startPos = draggable.transform.position;

            draggable.transform.SetParent(transform, false);

            // snap to grid
            draggable.transform.localPosition = Vector3.zero;

            // 
            Canvas.ForceUpdateCanvases();

            // capture snapped world position
            Vector3 endPos = draggable.transform.position;

            // reset back to original drop position
            draggable.transform.position = startPos;

            // tween into place
            draggable.transform.MoveToPosition(.5f, endPos);

            return true;
        }
    }

}
