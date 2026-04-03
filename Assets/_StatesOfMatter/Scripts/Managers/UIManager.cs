using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TMKOC.StatesOfMatter
{
    public class UIManager : GenericSingleton<UIManager>
    {
        [SerializeField] private GameObject m_winPanel, m_losePanel;

        [Header("Tutorial Components")]
        [Space]
        [SerializeField] private Image m_image;
        [SerializeField] private TextMeshProUGUI m_header, m_primaryText1, m_primaryText2, m_secondaryText;
        [SerializeField] private float typingSpeed = 0.025f;
        [SerializeField] private float initialDelay = 1f;
        [SerializeField] private Button m_nextBtn;
        [SerializeField] private GameObject m_particleParent;
        [SerializeField] private bool skipRequested = false;

        [SerializeField] private GameObject[] m_slideImageList;

        [Header("Canvas")]
        [SerializeField] private Canvas m_gameCanvas;
        [SerializeField] private Canvas m_tutorialCanvas;


        [SerializeField] private PlayschoolController m_playschoolController;

        private WaitForSeconds waitTimer;

        private void OnEnable()
        {
            GameManager.OnGameStart += DisablePanels;
            GameManager.OnGameRestart += DisablePanels;

            GameManager.OnTutorialStart += CheckCanvases;
            GameManager.OnTutorialEnd += ChangeCanvas;

            TutorialManager.OnNewDataLoaded += ChangeTutorialData;
            GameManager.OnGameEnd += EnableGameEndPanels;
            AnimationController.OnEndAnimationFinished += EnableGameEndPanels;
        }

        private void Start()
        {
            waitTimer = new WaitForSeconds(initialDelay);
        }

        private void OnDisable()
        {
            GameManager.OnGameStart -= DisablePanels;
            GameManager.OnGameRestart -= DisablePanels;

            GameManager.OnTutorialStart -= CheckCanvases;
            GameManager.OnTutorialEnd -= ChangeCanvas;
            TutorialManager.OnNewDataLoaded -= ChangeTutorialData;
            GameManager.OnGameEnd -= EnableGameEndPanels;
            AnimationController.OnEndAnimationFinished -= EnableGameEndPanels;
        }

        private bool endToggle = false;

        private void EnableGameEndPanels(bool toggle)
        {
            endToggle = toggle;
        }

        private void EnableGameEndPanels()
        {
            if (endToggle)
            {
                //m_playschoolController.EnableFinalWinPanelAfterDelay();
                m_playschoolController.EnableFinalWinPanelAfterDelay();
            }
            else
            {
                //m_losePanel.SetActive(!endToggle);

                WinLosePanelScript.Instance.ShowRetryPopUp(GameManager.Instance.InvokeGameRestart);
            }
        }

        private void DisablePanels()
        {
            m_winPanel.SetActive(false);
            //m_losePanel.SetActive(false);
        }

        private void ChangeCanvas()
        {
            m_tutorialCanvas.gameObject.SetActive(false);
            m_gameCanvas.gameObject.SetActive(true);
        }

        private void CheckCanvases()
        {
            m_tutorialCanvas.gameObject.SetActive(true);
            m_gameCanvas.gameObject.SetActive(false);
        }

        private void ChangeTutorialData(TutorialData data)
        {

            DisableSlideImages();

            m_nextBtn.gameObject.SetActive(false);

            m_header.SetText(string.Empty);
            m_primaryText1.SetText(string.Empty);
            m_primaryText2.SetText(string.Empty);
            m_secondaryText.SetText(string.Empty);

            StartCoroutine(DisplayTutorialData(data));
        }

        private IEnumerator DisplayTutorialData(TutorialData data)
        {
            yield return waitTimer;

            yield return TypeAndShow(m_header, data.Heading);
            yield return waitTimer;

            yield return TypeAndShow(m_primaryText1, data.PrimaryText1);
            yield return waitTimer;

            EnableSlideImage(data.Index);

            yield return TypeAndShow(m_primaryText2, data.PrimaryText2);
            yield return waitTimer;

            yield return TypeAndShow(m_secondaryText, data.SecondaryText);
            yield return waitTimer;

            m_nextBtn.gameObject.SetActive(true);
        }

        private IEnumerator TypeAndShow(TextMeshProUGUI tmpText, string fullText)
        {
            tmpText.gameObject.SetActive(true);
            tmpText.text = fullText;               // Set full text first, so TMP parses tags
            tmpText.ForceMeshUpdate();             // Make sure textInfo is updated

            int totalCharacters = tmpText.textInfo.characterCount;
            tmpText.maxVisibleCharacters = 0;      // Hide all initially

            for (int i = 0; i < totalCharacters; i++)
            {
                tmpText.maxVisibleCharacters++;

                char c = tmpText.textInfo.characterInfo[i].character;

                // Optional: add small delay for punctuation
                if (c == '.' || c == ',' || c == ':' || c == ';' ||
                    c == '?' || c == '!' || c == '-')
                {
                    yield return new WaitForSeconds(typingSpeed * 3f); // slightly longer pause
                }
                else
                {
                    yield return new WaitForSeconds(typingSpeed);
                }

                // If you implement a skip button:
                if (skipRequested) break;
            }

            tmpText.maxVisibleCharacters = totalCharacters; // Ensure fully visible
        }


        // Optional: Add method to instantly show full text
        public void SkipAllTyping()
        {
            StopAllCoroutines();
            m_header.gameObject.SetActive(true);
            m_primaryText1.gameObject.SetActive(true);
            m_primaryText2.gameObject.SetActive(true);
            m_secondaryText.gameObject.SetActive(true);
            m_nextBtn.gameObject.SetActive(true);
        }

        private void EnableSlideImage(int index)
        {
            if (index < 0 || index >= m_slideImageList.Length)
            {
                Debug.Log("Index out of bound");
                return;
            }

            m_slideImageList[index].gameObject.SetActive(true);
        }

        private void DisableSlideImages()
        {
            foreach (var slideImage in m_slideImageList)
            {
                slideImage.SetActive(false);
            }
        }
    }
}