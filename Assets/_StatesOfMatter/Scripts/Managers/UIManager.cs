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

        [SerializeField] private GameObject[] m_slideImageList;

        [Header("Canvas")]
        [SerializeField] private Canvas m_gameCanvas;
        [SerializeField] private Canvas m_tutorialCanvas;

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

        private void EnableGameEndPanels(bool toggle) => endToggle = toggle;

        private void EnableGameEndPanels()
        {
            m_winPanel.SetActive(endToggle);
            m_losePanel.SetActive(!endToggle);
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
            m_nextBtn.gameObject.SetActive(true);
        }

        private void EnableParticles()
        {
            m_particleParent.SetActive(true);
            //m_particleParent.transform.localScale = Vector3.zero;
            //m_particleParent.transform.DOScale(Vector3.one, 0.75f);
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

        private void DisableParticles()
        {
            m_particleParent.SetActive(false);
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