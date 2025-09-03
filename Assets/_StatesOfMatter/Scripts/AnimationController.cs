using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace TMKOC.StatesOfMatter
{
    public class AnimationController : MonoBehaviour
    {
        [SerializeField] private Vector3 moveInPos, moveOutPos;
        [SerializeField] private float moveTweenTime;

        private void Start()
        {
            MoveIn(GameManager.Instance.RequestNextItem);
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

        [Button]
        public void MoveOut()
        {
            transform.DOLocalMove(moveOutPos, moveTweenTime);
        }
    }
}