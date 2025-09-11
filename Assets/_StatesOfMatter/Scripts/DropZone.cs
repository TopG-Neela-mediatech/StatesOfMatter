using DG.Tweening;
using TMKOC.StatesOfMatter;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum StateType { Solid, Liquid, Gas }

public class DropZone : MonoBehaviour
{
    public static event System.Action DropZoneFull;

    [SerializeField] private StateType zoneCategory;

    [Header("Placement Mode")]
    [Tooltip("If true, will use explicit slots as children of the grid.")]
    [SerializeField] private bool useSlots = false;

    [SerializeField] private int totalCount = 5;

    [Tooltip("Explicit slots inside this zone (only used if UseSlots = true)")]
    [SerializeField] private RectTransform[] setPoints;

    private int currentIndex = 0;
    private GridLayoutGroup _grid;

    public StateType ZoneType { get { return zoneCategory; } }

    private void Awake()
    {
        _grid = GetComponent<GridLayoutGroup>();
        if (_grid == null)
        {
            Debug.LogWarning("DropZone works best with a GridLayoutGroup on the same GameObject.");
        }
    }

    private void OnEnable()
    {
        GameManager.OnGameRestart += ResetBoard;
    }

    private void OnDisable()
    {
        GameManager.OnGameRestart -= ResetBoard;
    }


    /// <summary>
    /// Places draggable either in the next free slot, or appends to the grid.
    /// Returns true if placed successfully.
    /// </summary>
    public bool PlaceDraggable(MatterDraggable draggable)
    {
        if (useSlots && setPoints != null && setPoints.Length > 0)
        {
            Transform slot = setPoints[currentIndex++];
            if (slot.childCount == 0)
            {
                draggable.transform.SetParent(slot, true);
                draggable.transform.DOLocalMove(Vector3.zero, .5f).OnComplete(() =>
                {
                    draggable.ToggleTextObj(true);
                });
                CheckSlotCount();
                return true;
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

            Canvas.ForceUpdateCanvases();

            // capture snapped world position
            Vector3 endPos = draggable.transform.position;

            // reset back to original drop position
            draggable.transform.position = startPos;

            // tween into place
            draggable.transform.DOMove(endPos, .5f).OnComplete(() =>
            {
                draggable.ToggleTextObj(true);
            });

            return true;
        }
    }

    private void CheckSlotCount()
    {
        Debug.Log("Current index: " + currentIndex);
        Debug.Log("setPt Len-1 for 0 indexing: " + (setPoints.Length - 1));

        if (currentIndex >= setPoints.Length)
        {
            // full hogaya slots
            // can end game here
            DropZoneFull?.Invoke();
        }
    }

    private void ResetBoard()
    {
        currentIndex = 0;
        ClearSetPoints();
    }

    private void ClearSetPoints()
    {
        for (int i = 0; i < setPoints.Length; i++)
        {
            if(setPoints[i] != null && setPoints[i].childCount > 0)
            {
                Transform go = setPoints[i].GetChild(0);
                Destroy(go.gameObject);
            }
        }
    }
}
