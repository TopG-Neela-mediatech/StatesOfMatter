using UnityEngine;

namespace TMKOC.StatesOfMatter
{

    public class UIManager : GenericSingleton<UIManager>
    {
        [SerializeField] private GameObject m_winPanel, m_losePanel;

        private void OnEnable()
        {
            GameManager.OnGameEnd += EnableGameEndPanels;
            GameManager.OnGameStart += DisablePanels;
            GameManager.OnGameRestart += DisablePanels;
        }

        private void OnDisable()
        {
            GameManager.OnGameEnd -= EnableGameEndPanels;
            GameManager.OnGameStart -= DisablePanels;
            GameManager.OnGameRestart -= DisablePanels;
        }

        private void EnableGameEndPanels(bool toggle)
        {
            m_winPanel.SetActive(toggle);
            m_losePanel.SetActive(!toggle);
        }

        private void DisablePanels()
        {
            m_winPanel.SetActive(false);
            m_losePanel.SetActive(false);
        }
    }

}