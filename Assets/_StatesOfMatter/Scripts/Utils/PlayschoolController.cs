using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayschoolController : MonoBehaviour
{
    private GameCategoryDataManager _gameCategoryDataManager;
    private UpdateCategoryApiManager _updateCategoryManager;
    private UpdateCategoryApiManagerTest _updateCategoryApiManagerTest;

    [SerializeField] private int _gameID;
    [SerializeField] private int _testID;
    [SerializeField] private GameObject finalTestPanel;

    [SerializeField] private int _totalCountLevel = 5;


    private void Awake()
    {
        HelperGameCategoryDataSaver.Init(_totalCountLevel);

        HelperGameCategoryDataSaver.LevelCompleted(0);
    }

    public void OnBackButtonPressed()
    {
        SceneManager.LoadScene(TMKOCPlaySchoolConstants.TMKOCPlayMainMenu);
    }

    public int GetStarsAcquired()
    {
        if (_gameCategoryDataManager != null)
            return _gameCategoryDataManager.GetLoadedstar;
        else
            return 0;
    }

    public void SaveStarOnFinish()
    {
#if PLAYSCHOOL_MAIN

            // this _star the get loaded star from json
            
            int _star = _gameCategoryDataManager.GetLoadedstar;

            if (_star == 5)
            {
                // After One Time full game completed Logic
                _updateCategoryManager.SetGameDataMore(1, 1, 5);
                //EnableFinalWinPanelAfterDelay();
            }
            else
            {
                //Playing Game for First Time
                EnableFinalWinPanelAfterDelay();
                _updateCategoryManager.SetGameDataMore(1, 1, _star);
            }
            return;

#endif
        EnableFinalWinPanelAfterDelay();
        TestFinished();
    }

    public void EnableFinalWinPanelAfterDelay()
    {
#if PLAYSCHOOL_MAIN
                    EffectParticleControll.Instance.SpawnGameEndPanel();
                    GameOverEndPanel.Instance.AddTheListnerRetryGame();
#else
        //Your testing End panel
        finalTestPanel.SetActive(true);
#endif
    }

    private void TestFinished()
    {
        // SaveTestStar(correctAnswers, totalQuestions) 
        _gameCategoryDataManager.SaveTestStar(1, 1);
        int testStar = _gameCategoryDataManager.GetTeststar;
        // SaveTestStar(Score(I Will be remove after some time score), totalQuestions, testStar)
        _updateCategoryApiManagerTest.SetGameDataMore(1, 1, testStar);
    }

}
