using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace TMKOC.StatesOfMatter
{
    public class AnimationController : MonoBehaviour
    {
        [SerializeField] private Vector3 moveInPos, moveOutPos;
        [SerializeField] private float moveTweenTime;

        private bool IsLivesOver = false;

        private void OnEnable()
        {
            GameManager.OnGameStart += MoveInAndSpawn;
            GameManager.OnGameRestart += OnRestart;

            AdvancedDraggable.OnDragStart += MoveOut;
            LivesManager.OnLivesOver += OnLivesOver;

            GameManager.OnCorrectDrop += MoveInAndSpawn;
            GameManager.OnIncorrectDrop += MoveInAndSpawn;
        }

        private void OnDisable()
        {
            GameManager.OnGameStart -= MoveInAndSpawn;
            GameManager.OnGameRestart -= OnRestart;

            AdvancedDraggable.OnDragStart -= MoveOut;
            LivesManager.OnLivesOver -= OnLivesOver;

            GameManager.OnCorrectDrop -= MoveInAndSpawn;
            GameManager.OnIncorrectDrop -= MoveInAndSpawn;
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

        private void OnRestart()
        {
            IsLivesOver = false;
            MoveInAndSpawn();
        }

        private void MoveInAndSpawn()
        {
            transform.DOLocalMove(moveInPos, moveTweenTime).OnComplete(() =>
            {
                if (!IsLivesOver) GameManager.Instance.RequestRandomItem(); 
                else MoveOut();
            });
        }

        [Button]
        public void MoveOut()
        {
            transform.DOLocalMove(moveOutPos, moveTweenTime).SetEase(Ease.InQuad);
        }

        private void MoveOut(AdvancedDraggable draggable)
        {
            transform.DOLocalMove(moveOutPos, moveTweenTime);
        }

        private void OnLivesOver() => IsLivesOver = true;

    }
}