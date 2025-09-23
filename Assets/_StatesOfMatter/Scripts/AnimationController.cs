using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace TMKOC.StatesOfMatter
{
    public class AnimationController : MonoBehaviour
    {
        [SerializeField] private Vector3 moveInPos, moveOutPos;
        [SerializeField] private float moveTweenTime;

        private bool IsLivesOver = false;

        private Animator m_animator;

        private readonly int Idle = Animator.StringToHash("Idle");
        private readonly int Correct = Animator.StringToHash("Correct");
        private readonly int Incorrect = Animator.StringToHash("Incorrect");


        public static event Action OnEndAnimationFinished;

        private void Awake()
        {
            m_animator = GetComponent<Animator>();
            m_animator.Play(Idle);
        }

        private void OnEnable()
        {
            GameManager.OnGameEnd += PlayEndingAnimation;
            DropZone.DropZoneFull += PlayEndWinAnimation;

            GameManager.OnGameStart += MoveInAndSpawn;
            GameManager.OnGameRestart += OnRestart;

            AdvancedDraggable.OnDragStart += MoveOut;
            LivesManager.OnLivesOver += OnLivesOver;

            GameManager.OnCorrectDrop += MoveInAndSpawn;
            GameManager.OnIncorrectDrop += MoveInAndSpawn;

        }

        private void OnDisable()
        {
            GameManager.OnGameEnd -= PlayEndingAnimation;
            DropZone.DropZoneFull -= PlayEndWinAnimation;

            GameManager.OnGameStart -= MoveInAndSpawn;
            GameManager.OnGameRestart -= OnRestart;

            AdvancedDraggable.OnDragStart -= MoveOut;
            LivesManager.OnLivesOver -= OnLivesOver;

            GameManager.OnCorrectDrop -= MoveInAndSpawn;
            GameManager.OnIncorrectDrop -= MoveInAndSpawn;

        }

        private void OnRestart()
        {
            IsLivesOver = false;
            isGameOver = false;
            m_animator.Play(Idle);
            MoveInAndSpawn();
        }

        private bool isGameOver = false;

        private void MoveInAndSpawn()
        {
            transform.DOLocalMove(moveInPos, moveTweenTime).OnComplete(() =>
            {
                if (isGameOver) return;
                if (!IsLivesOver) GameManager.Instance.RequestRandomItem();
                else MoveOut();
            });
        }

        private void MoveOut(AdvancedDraggable draggable)
        {
            transform.DOLocalMove(moveOutPos, moveTweenTime);
        }

        private void OnLivesOver() => IsLivesOver = true;

        private void PlayEndingAnimation(bool isWon)
        {
            AnimatorStateInfo info;
            isGameOver = true;

            if (!isWon)
            {
                m_animator.Play(Incorrect);
                info = m_animator.GetCurrentAnimatorStateInfo(0);
                StartCoroutine(DelayedInvoke(info.length));
            }
        }

        private void PlayEndWinAnimation()
        {
            AnimatorStateInfo info;
            isGameOver = true;

            transform.DOLocalMove(moveInPos, moveTweenTime).OnComplete(() =>
            {
                m_animator.Play(Correct);
                info = m_animator.GetCurrentAnimatorStateInfo(0);

                StartCoroutine(DelayedInvoke(info.length));
            });

        }

        private IEnumerator DelayedInvoke(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);

            OnEndAnimationFinished?.Invoke();

            MoveOut();
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
            transform.DOLocalMove(moveOutPos, moveTweenTime).SetEase(Ease.InQuad);
        }
    }
}