using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TMKOC.StatesOfMatter
{
    public class GameManager : GenericSingleton<GameManager>
    {
        [SerializeField] private MatterSO matterSO;
        [SerializeField] private float _startGameWaitTime;

        [SerializeField] private bool _skipTutorial = false;

        private List<ItemData> matterList;
        private int currentIndex = 0;
        private bool IsGameFinished = false;

        public float StartWaitTime { get { return _startGameWaitTime; } }

        #region Monobehaviour Funcs

        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            PlayschoolCommon.Instance.SpawnplayschoolWinLosePanel();

            CopyMatterList();

            if (_skipTutorial)
            {
                SkipTutorial();
                return;
            }
            OnTutorialStart?.Invoke();
        }

        private void OnEnable()
        {
            DropZone.DropZoneFull += GameFinished;
            LivesManager.OnLivesOver += InvokeGameEnd;
        }

        private void OnDisable()
        {
            DropZone.DropZoneFull -= GameFinished;
            LivesManager.OnLivesOver -= InvokeGameEnd;
        }

        #endregion

        #region Game events

        public static event Action OnTutorialStart, OnTutorialEnd;
        public static event Action OnNextTutorialRequested;

        public static event Action OnGameWin, OnGameLose;
        public static event Action OnGameStart, OnGameRestart;
        public static event Action<bool> OnGameEnd;
        public static event Action<ItemData> OnRequestNextItem;
        public static event Action OnCorrectDrop, OnIncorrectDrop;

        public void OnCorrectDrag()
        {

            if (IsGameFinished)
            {
                OnGameEnd?.Invoke(true);
            }
            else
            {
                OnCorrectDrop?.Invoke();
            }
        }

        public void OnIncorrectDrag()
        {
            OnIncorrectDrop?.Invoke();
        }

        public void InvokeGameRestart()
        {
            OnGameRestart?.Invoke();

            HandleRestart();

            HelperGameCategoryDataSaver.AddAttempt(); //Save Add Attempt
        }

        public void InvokeNextTutorial()
        {
            OnNextTutorialRequested?.Invoke();
        }

        public void InvokeTutorialEnd()
        {
            OnTutorialEnd?.Invoke();

            StartCoroutine(DelayedStart(0.25f));
        }

        public void GoBackToPlayschool()
        {
            SceneManager.LoadScene(TMKOCPlaySchoolConstants.TMKOCPlayMainMenu);
        }

        private IEnumerator DelayedStart(float delay)
        {
            yield return new WaitForSeconds(delay);

            OnGameStart?.Invoke();
        }

        public void InvokeGameEnd(bool toggle)
        {
            OnGameEnd?.Invoke(toggle);
        }

        private void InvokeGameEnd()
        {
            OnGameEnd?.Invoke(false);
        }

        #endregion

        private void HandleRestart()
        {
            IsGameFinished = false;
            CopyMatterList();
        }

        private void CopyMatterList()
        {
            matterList = new List<ItemData>(matterSO.Length);

            foreach (var item in matterSO.ItemList)
            {
                matterList.Add(item);
            }
        }

        private void GameFinished()
        {
            IsGameFinished = true;
        }

        public void RequestRandomItem()
        {
            if (matterList.Count == 0)
            {
                CopyMatterList();
            }

            var randomIndex = UnityEngine.Random.Range(0, matterList.Count);

            var item = matterList[randomIndex];
            OnRequestNextItem?.Invoke(item);

            matterList.RemoveAt(randomIndex);
        }

        public void RequestNextItem()
        {
            if (currentIndex >= matterSO.Length)
            {
                Debug.Log("All items spawned!");
                return;
            }

            var item = matterSO.GetItem(currentIndex);
            OnRequestNextItem?.Invoke(item);
            currentIndex++;
        }

        public void SkipTutorial()
        {
            OnTutorialEnd?.Invoke();
            OnGameStart?.Invoke();
        }

    }
}