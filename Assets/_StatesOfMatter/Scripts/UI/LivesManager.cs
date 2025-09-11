using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace TMKOC.StatesOfMatter
{
    public class LivesManager : MonoBehaviour
    {
        [SerializeField] private GameObject _livesParent;
        [SerializeField] private Image[] _filledHeartImages;
        [SerializeField] private GameObject _losePanel;
        [SerializeField] private GameObject heartBreakEffect;
        [SerializeField] private GameObject _tickButton;
        [SerializeField] private float yOffset = 45f;
        private int _lives = 3;

        private void ResetLives() => _lives = 3;

        public static event Action OnLivesOver, OnLivesReduced;

        private void OnEnable()
        {
            GameManager.OnIncorrectDrop += ReduceLive;
            GameManager.OnGameStart += EnableLives;
            GameManager.OnGameRestart += EnableLives;
        }

        private void OnDisable()
        {
            GameManager.OnIncorrectDrop -= ReduceLive;
            GameManager.OnGameStart -= EnableLives;
            GameManager.OnGameRestart -= EnableLives;
        }


        [Button]
        private void EnableLives()
        {
            ResetLives();
            foreach (var item in _filledHeartImages)
            {
                item.enabled = true;
            }
        }
        private void DeScaleLiveParent()
        {
            _livesParent.transform.DOScale(Vector3.zero, 0.3f);
        }

        private void ScaleLiveParent()
        {
            _livesParent.transform.DOScale(Vector3.one, 1f);
        }


        [Button]
        public void ReduceLive()
        {
            StartCoroutine(ReduceLiveAfterDelay());
        }

        private IEnumerator ReduceLiveAfterDelay()
        {
            if (_tickButton != null)
                _tickButton.SetActive(false);

            if (_lives <= 0)
            {
                OnLivesOver?.Invoke();
                if (_losePanel != null)
                    _losePanel.SetActive(true);
                _lives = 0;

                // Play heart breaking audio

                yield break;
            }

            _lives--;

            SetParticleEffectPosition(_lives);
            heartBreakEffect.SetActive(true);

            yield return new WaitForSeconds(.5f);

            heartBreakEffect.SetActive(false);
            _filledHeartImages[_lives].enabled = false;

            OnLivesReduced?.Invoke();

            if (_lives <= 0)
            {
                _lives = 0;
                OnLivesOver?.Invoke();
                yield return new WaitForSeconds(0.8f);
            }

            if (_tickButton != null)
                _tickButton.SetActive(true);
        }

        private void SetParticleEffectPosition(int liveNumber)
        {
            if (liveNumber == 2)
            {
                heartBreakEffect.transform.localPosition = new Vector3(90f,yOffset, 0f);
            }
            if (liveNumber == 1)
            {
                heartBreakEffect.transform.localPosition = new Vector3(0f, yOffset, 0f);
            }
            if (liveNumber == 0)
            {
                heartBreakEffect.transform.localPosition = new Vector3(-90f, yOffset, 0f);
            }
        }
    }
}