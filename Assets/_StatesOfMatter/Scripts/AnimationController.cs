using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace TMKOC.StatesOfMatter
{
    public class AnimationController : MonoBehaviour
    {
        [SerializeField] private Vector3 moveInPos, moveOutPos;
        [SerializeField] private float moveTweenTime;

        private void OnEnable()
        {
            GameManager.OnGameStart += MoveInAndSpawn;
            AdvancedDraggable.OnDragStart += MoveOut;

            GameManager.OnCorrectDrop += MoveInAndSpawn;
            GameManager.OnIncorrectDrop += MoveInAndSpawn;
        }

        [Button]
        public void MoveIn()
        {
            transform.DOLocalMove(moveInPos, moveTweenTime);
        }
        public void MoveIn(System.Action callback)
        {
            transform.DOLocalMove(moveInPos, moveTweenTime).OnComplete(() =>
            {
                callback?.Invoke();
            });
        }
        public void MoveInAndSpawn()
        {
            transform.DOLocalMove(moveInPos, moveTweenTime).OnComplete(() =>
            {
                GameManager.Instance.RequestRandomItem();
            });
        }



        [Button]
        public void MoveOut()
        {
            transform.DOLocalMove(moveOutPos, moveTweenTime);
        }

        private void MoveOut(AdvancedDraggable draggable)
        {
            transform.DOLocalMove(moveOutPos, moveTweenTime);
        }
    }
}