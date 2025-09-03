using UnityEngine;
using UnityEngine.EventSystems;
using System;
using DG.Tweening;
using UnityEngine.UI;

namespace TMKOC.StatesOfMatter
{
    public enum DropType
    {
        None,
        Correct,
        Incorrect,
        OutsideDropZone,
    }

    public class AdvancedDraggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private Vector3 _initialPosition;
        private Transform _initialParent;
        private Canvas _parentCanvas;
        private CanvasGroup _canvasGroup;

        // Events (you can subscribe externally)
        public static event Action<AdvancedDraggable> OnDragStart;
        public static event Action<AdvancedDraggable, Vector3> OnDragging;
        public static event Action<AdvancedDraggable, GameObject> OnDragEnd;

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
                DropType dropType = IsValidDropTarget(dropTarget);
                DropResult(dropType);
            }
        }

        private void DropResult(DropType dropType)
        {
            switch (dropType)
            {
                case DropType.Correct:
                    GameManager.Instance.OnCorrectDrag();
                    break;
                case DropType.Incorrect:
                    GameManager.Instance.OnIncorrectDrag();
                    KillDraggable();
                    break;
                case DropType.OutsideDropZone:
                    Reset();
                    break;
            }
        }

        /// <summary>
        /// Incomplete Base function
        /// Override this method for custom drop validation.
        /// </summary>
        protected virtual DropType IsValidDropTarget(GameObject target)
        {
            // Example: Only accept colliders with tag "DropZone"
            if (target == null) return DropType.OutsideDropZone;

            // 
            return DropType.None;
        }

        public void ResetToInitialPosition()
        {
            transform.DOMove(_initialPosition, moveTime);
        }

        public void Reset()
        {
            transform.SetParent(_initialParent);
            IsDraggable = true;

            if (resetIfNotDroppedCorrectly)
            {
                ResetToInitialPosition();
            }
        }

        public void KillDraggable()
        {
            Image image = GetComponent<Image>();
            if (image != null)
            {
                image.DOColor(Color.clear, 0.75f).OnComplete(() =>
                {
                    Destroy(this.gameObject);
                });
            }
        }
    }
}