using System;
using UnityEngine;

namespace TMKOC.StatesOfMatter
{
    public class TutorialManager : MonoBehaviour
    {
        [SerializeField] private TutorialDataSO m_tutorialDataList;

        private int currentIndex = 0;

        public static event Action<TutorialData> OnNewDataLoaded;
        public static event Action OnParticleSlideLoaded;


        private void OnEnable()
        {
            GameManager.OnTutorialStart += StartTutorial;

            GameManager.OnNextTutorialRequested += DisplayNextData;
        }

        private void OnDisable()
        {
            GameManager.OnTutorialStart -= StartTutorial;

            GameManager.OnNextTutorialRequested -= DisplayNextData;
        }

        private void StartTutorial()
        {
            currentIndex = 0;

            DisplayNextData();
        }

        private void DisplayNextData()
        {
            if (currentIndex >= m_tutorialDataList.Length)
            {
                GameManager.Instance.InvokeTutorialEnd();
                return;
            }

            // get the data
            TutorialData tutData = m_tutorialDataList.GetData(currentIndex);

            if (tutData != null)
            {
                tutData.Index = currentIndex;
                OnNewDataLoaded?.Invoke(tutData);
            }

            currentIndex++;
        }
    }
}