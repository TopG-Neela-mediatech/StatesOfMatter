using UnityEngine;
using UnityEngine.EventSystems;
using System;
using DG.Tweening;

public class AdvancedDraggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Vector3 _initialPosition;
    private Transform _initialParent;
    private Canvas _parentCanvas;
    private CanvasGroup _canvasGroup;

    // Events (you can subscribe externally)
    public event Action<AdvancedDraggable> OnDragStart;
    public event Action<AdvancedDraggable, Vector3> OnDragging;
    public event Action<AdvancedDraggable, GameObject> OnDragEnd;

    // Optional flag if you want snapping / reset logic handled internally
    [SerializeField] private bool resetIfNotDroppedCorrectly = true;
    [SerializeField] protected bool IsDraggable;
    [SerializeField] protected float moveTime = 0.75f;

    private void Awake()
    {
        _parentCanvas = GetComponentInParent<Canvas>();

        _canvasGroup = GetComponentInParent<CanvasGroup>();
    }

    private void Start()
    {
        if (_canvasGroup == null)
        {
            Debug.LogWarning("Please add a canvas group to the draggables");
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (IsDraggable)
        {
            _canvasGroup.blocksRaycasts = false;

            _initialPosition = transform.position;
            _initialParent = transform.parent;

            // Bring to top layer so it doesn’t hide under UI
            transform.SetAsLastSibling();

            OnDragStart?.Invoke(this);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (IsDraggable)
        {
            if (_parentCanvas == null) return;

            RectTransformUtility.ScreenPointToWorldPointInRectangle(
                _parentCanvas.transform as RectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out Vector3 globalMousePos
            );

            transform.position = globalMousePos;

            OnDragging?.Invoke(this, transform.position);
        }
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        if (IsDraggable)
        {
            _canvasGroup.blocksRaycasts = true;
            IsDraggable = false;

            GameObject dropTarget = eventData.pointerEnter; // Object under pointer (if any)

            OnDragEnd?.Invoke(this, dropTarget);

            // If no target or wrong target, reset
            if (resetIfNotDroppedCorrectly && !IsValidDropTarget(dropTarget))
            {
                ResetToInitialPosition();
            }
        }
    }

    /// <summary>
    /// Override this method for custom drop validation.
    /// </summary>
    protected virtual bool IsValidDropTarget(GameObject target)
    {
        // Example: Only accept colliders with tag "DropZone"
        if (target == null) return false;
        return target.CompareTag("DropZone");
    }

    public void ResetToInitialPosition()
    {
        transform.SetParent(_initialParent);

        transform.MoveToPosition(moveTime, _initialPosition);

        IsDraggable = true;
    }

    //public void MoveToLocalPosition(Vector3 endPosition)
    //{
    //    transform.DOLocalMove(endPosition, moveTime);
    //}
}
