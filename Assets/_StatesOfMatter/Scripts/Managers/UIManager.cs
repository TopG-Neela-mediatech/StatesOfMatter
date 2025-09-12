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

        [Header("Canvas")]
        [SerializeField] private Canvas m_gameCanvas;
        [SerializeField] private Canvas m_tutorialCanvas;

        private WaitForSeconds waitTimer;

        private void OnEnable()
        {
            GameManager.OnGameEnd += EnableGameEndPanels;
            GameManager.OnGameStart += DisablePanels;
            GameManager.OnGameRestart += DisablePanels;

            GameManager.OnTutorialStart += CheckCanvases;
            GameManager.OnTutorialEnd += ChangeCanvas;
            TutorialManager.OnNewDataLoaded += ChangeTutorialData;
        }

        private void Start()
        {
            waitTimer = new WaitForSeconds(initialDelay);
        }

        private void OnDisable()
        {
            GameManager.OnGameEnd -= EnableGameEndPanels;
            GameManager.OnGameStart -= DisablePanels;
            GameManager.OnGameRestart -= DisablePanels;

            GameManager.OnTutorialStart -= CheckCanvases;
            GameManager.OnTutorialEnd -= ChangeCanvas;
            TutorialManager.OnNewDataLoaded -= ChangeTutorialData;
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
            m_header.SetText(data.Heading);
            m_primaryText1.SetText(data.PrimaryText1);
            m_primaryText2.SetText(data.PrimaryText2);
            m_secondaryText.SetText(data.SecondaryText);
            m_image.sprite = data.Sprite;
            m_image.color = Color.white;

            // Hide all text fields initially
            m_header.gameObject.SetActive(false);
            m_primaryText1.gameObject.SetActive(false);
            m_primaryText2.gameObject.SetActive(false);
            m_secondaryText.gameObject.SetActive(false);

            StartCoroutine(DisplayTutorialData());
        }

        private IEnumerator DisplayTutorialData()
        {
            yield return waitTimer;

            yield return TypeAndShow(m_header);
            yield return waitTimer;

            yield return TypeAndShow(m_primaryText1);
            yield return waitTimer;

            yield return TypeAndShow(m_primaryText2);
            yield return waitTimer;

            yield return TypeAndShow(m_secondaryText);
        }

        private IEnumerator TypeAndShow(TextMeshProUGUI tmpText)
        {
            tmpText.gameObject.SetActive(true);

            string fullText = tmpText.text;
            tmpText.text = "";

            int i = 0;
            string displayedText = "";
            string openTags = "";

            while (i < fullText.Length)
            {
                if (fullText[i] == '<')
                {
                    // Start of a tag: Capture entire tag and keep it in openTags
                    int tagEnd = fullText.IndexOf('>', i);
                    if (tagEnd == -1)
                    {
                        // Malformed tag, break
                        break;
                    }

                    string tag = fullText.Substring(i, tagEnd - i + 1);
                    openTags += tag;
                    i = tagEnd + 1;
                }
                else
                {
                    // Append next character with all accumulated open tags
                    displayedText += fullText[i];
                    tmpText.text = openTags + displayedText;
                    i++;

                    yield return new WaitForSeconds(typingSpeed);
                }
            }

            // Ensure full text is displayed at the end
            tmpText.text = fullText;
        }

        // Optional: Add method to instantly show full text
        public void SkipAllTyping()
        {
            StopAllCoroutines();
            m_header.gameObject.SetActive(true);
            m_primaryText1.gameObject.SetActive(true);
            m_primaryText2.gameObject.SetActive(true);
            m_secondaryText.gameObject.SetActive(true);
        }
    }
}
