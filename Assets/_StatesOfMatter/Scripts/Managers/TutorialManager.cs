using System;
using TMKOC.StatesOfMatter;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private TutorialDataSO m_tutorialDataList;

    private int currentIndex = 0;

    public static event Action<TutorialData> OnNewDataLoaded;


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
            // invoke the event to ui manager to collect the data
            // or take it directly as UIMan is singleton instance

            OnNewDataLoaded?.Invoke(tutData);
        }

        currentIndex++;
    }
}
