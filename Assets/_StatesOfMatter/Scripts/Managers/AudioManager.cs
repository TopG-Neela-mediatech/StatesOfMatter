using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMKOC.StatesOfMatter
{
    public enum SoundLanguage
    {
        None,
        English,
        Hindi,
        Tamil,
        French,
        Bengali,
        Marathi,
        Punjabi,
        Malayalam,
    }

    public enum TutorialAudio
    {
        OnGameStart,
        OnGameStop,
        SlideSpecific,
        Incorrect,
        BGMusic,
        Correct,
        Retry,
    }

    /// <summary>
    /// Manages game SFX and localized voice/dialog audio.
    /// </summary>
    public class AudioManager : GenericSingleton<AudioManager>
    {
        [Header("General Settings")]
        [SerializeField] private bool _isMute = false;
        [SerializeField] private float _intialTutorialDelay = 0.25f;
        [SerializeField] private float _regularTutorialDelay = 0.5f;
        [SerializeField] private float _gameStartDelay = 0.25f;
        [SerializeField] private float _gameEndDelay = 0.75f;
        [SerializeField] private float _correctDelay = 0.25f;
        [SerializeField] private float _incorrectDelay = 0.25f;

        [Header("Language Specific AudioSO")]
        [SerializeField] private AudioSO _englishAudioSO;
        [SerializeField] private AudioSO _hindiAudioSO;
        [SerializeField] private AudioSO _tamilAudioSO;
        [SerializeField] private AudioSO _frenchAudioSO;
        [SerializeField] private AudioSO _bengaliAudioSO;
        [SerializeField] private AudioSO _marathiAudioSO;
        [SerializeField] private AudioSO _punjabiAudioSO;
        [SerializeField] private AudioSO _malayalamAudioSO;

        [Header("Audio Sources")]
        [SerializeField] private AudioSource _bgSource;
        [SerializeField] private AudioSource _voiceSource;
        [SerializeField] private AudioSource _sfxSource;

        [Header("BG Samples")]
        [SerializeField] private AudioClip bgSound;

        private AudioSO _currentVoiceAudioSO;
        private float _defaultBGVolume;
        private float _defaultSFXVolume;
        private float _defaultVoiceVolume;

        private int _slideAudioIndex;
        private Dictionary<SoundLanguage, AudioSO> _languageAudioMap;
        private Coroutine _currentVoiceCoroutine;
        private Tween _sfxFadeTween;

        public SoundLanguage CurrentAudioLanguage { get; private set; }

        #region Unity Callbacks

        protected override void Awake()
        {
            base.Awake();
            InitializeLanguageAudio();

            if (!_bgSource || !_sfxSource || !_voiceSource)
                Debug.LogWarning("One or more AudioSources are not assigned in AudioManager!");

            PlayBGMusic();
        }

        private void Start()
        {
            // Store default volumes
            _defaultBGVolume = _bgSource.volume;
            _defaultSFXVolume = _sfxSource.volume;
            _defaultVoiceVolume = _voiceSource.volume;
        }

        private void OnEnable()
        {
            GameManager.OnTutorialStart += PlayFirstTutorialAudio;
            GameManager.OnNextTutorialRequested += PlayTutorialVoiceover;
            GameManager.OnGameStart += PlayGameStartAudio;
            GameManager.OnCorrectDrop += PlayCorrectAudio;
            GameManager.OnIncorrectDrop += PlayIncorrectAudio;
            GameManager.OnGameEnd += PlayGameEndAudio;
        }

        private void OnDisable()
        {
            GameManager.OnTutorialStart -= PlayFirstTutorialAudio;
            GameManager.OnNextTutorialRequested -= PlayTutorialVoiceover;
            GameManager.OnGameStart -= PlayGameStartAudio;
            GameManager.OnCorrectDrop -= PlayCorrectAudio;
            GameManager.OnIncorrectDrop -= PlayIncorrectAudio;
        }

        #endregion

        #region Language Handling

        private void InitializeLanguageAudio()
        {
            _languageAudioMap = new Dictionary<SoundLanguage, AudioSO>
            {
                { SoundLanguage.English, _englishAudioSO },
                { SoundLanguage.Hindi, _hindiAudioSO },
                { SoundLanguage.Tamil, _tamilAudioSO },
                { SoundLanguage.French, _frenchAudioSO },
                { SoundLanguage.Bengali, _bengaliAudioSO },
                { SoundLanguage.Marathi, _marathiAudioSO },
                { SoundLanguage.Punjabi, _punjabiAudioSO },
                { SoundLanguage.Malayalam, _malayalamAudioSO }
            };

#if PLAYSCHOOL_MAIN
            string langKey = string.Empty;
#else
            string langKey = PlayerPrefs.GetString("PlayschoolLanguageAudio", "English");
#endif
            if (!Enum.TryParse(langKey, out SoundLanguage lang))
                lang = SoundLanguage.English;

            CurrentAudioLanguage = lang;
            _currentVoiceAudioSO = _languageAudioMap.ContainsKey(lang) ? _languageAudioMap[lang] : _englishAudioSO;
        }

        #endregion

        #region Voice Controls


        /// <summary>
        /// Plays a localized voice/dialog clip.
        /// </summary>
        /// <param name="tutorialAudio">The audio enum to play</param>
        /// <param name="index">Optional index for specific clip</param>
        /// <param name="overrideIfPlaying">If false, will not interrupt current audio; default is true</param>
        public float PlayVoiceover(TutorialAudio tutorialAudio, int index = -1, bool overrideIfPlaying = true)
        {
            if (_isMute || _currentVoiceAudioSO == null)
                return float.NaN;

            // If audio is already playing and overrideIfPlaying is false, do nothing
            if (!overrideIfPlaying && _voiceSource.isPlaying)
                return _voiceSource.clip != null ? _voiceSource.clip.length : 0f;

            // Get clip
            var clip = index == -1
                ? _currentVoiceAudioSO.GetRandomAudioClip(tutorialAudio)
                : _currentVoiceAudioSO.GetSpecificAudioClip(tutorialAudio, index);

            if (clip == null)
                return float.NaN;

            _voiceSource.clip = clip;
            _voiceSource.Play();
            return clip.length;
        }

        private IEnumerator PlayVoiceoverWithDelay(TutorialAudio tutorialAudio, float delay, int index = -1, bool overrideFlag = true)
        {
            yield return new WaitForSeconds(delay);
            PlayVoiceover(tutorialAudio, index, overrideFlag);
        }

        private void PlayVoiceoverCoroutine(TutorialAudio tutorialAudio, float delay, int index = -1, bool overrideFlag = true)
        {
            // Stop any previously queued voiceover; new one may or may not play
            if (_currentVoiceCoroutine != null)
                StopCoroutine(_currentVoiceCoroutine);

            // Start coroutine, it will respect overrideFlag
            _currentVoiceCoroutine = StartCoroutine(PlayVoiceoverWithDelay(tutorialAudio, delay, index, overrideFlag));
        }

        private void PlayFirstTutorialAudio()
        {
            _slideAudioIndex = 0;
            PlayVoiceoverCoroutine(TutorialAudio.SlideSpecific, _intialTutorialDelay, _slideAudioIndex);
        }

        private void PlayTutorialVoiceover()
        {
            _slideAudioIndex++;
            if (_currentVoiceAudioSO.IsValidIndex(_slideAudioIndex))
            {
                PlayVoiceoverCoroutine(TutorialAudio.SlideSpecific, _regularTutorialDelay, _slideAudioIndex);
            }
        }

        private void PlayGameStartAudio()
        {
            PlayVoiceoverCoroutine(TutorialAudio.OnGameStart, _gameStartDelay);
        }

        private void PlayCorrectAudio()
        {
            PlayVoiceoverCoroutine(TutorialAudio.Correct, _correctDelay);
        }

        private void PlayIncorrectAudio()
        {
            PlayVoiceoverCoroutine(TutorialAudio.Incorrect, _incorrectDelay);
        }
        private void PlayGameEndAudio(bool isWon)
        {
            if (!isWon)
            {
                PlayVoiceoverCoroutine(TutorialAudio.Retry, _gameEndDelay, -1, false);
            }
            else
            {
                PlayVoiceoverCoroutine(TutorialAudio.OnGameStop, _gameEndDelay);
            }
        }

        private void PlayBGMusic()
        {
            if (!_isMute && bgSound)
            {
                _bgSource.clip = bgSound;
                _bgSource.Play();
            }
        }

        #endregion

        #region SFX & Global Controls

        public bool IsSFXPlaying() => _sfxSource.isPlaying;
        public bool IsVoicePlaying() => _voiceSource.isPlaying;

        public void StopAllAudio()
        {
            _bgSource.Stop();
            _sfxSource.Stop();
            _voiceSource.Stop();
        }

        public void StopAllSFX()
        {
            _sfxSource.Stop();
            _voiceSource.Stop();
        }

        public void StopVoice() => _voiceSource.Stop();

        public void FadeOutSFX(float endValue, float duration)
        {
            _sfxFadeTween?.Kill();
            _sfxFadeTween = _sfxSource.DOFade(endValue, duration).OnComplete(() => _sfxSource.Stop());
        }

        public void ToggleMute(bool mute)
        {
            _isMute = mute;
            _bgSource.volume = mute ? 0f : _defaultBGVolume;
            _sfxSource.volume = mute ? 0f : _defaultSFXVolume;
            _voiceSource.volume = mute ? 0f : _defaultVoiceVolume;
        }

        #endregion
    }
}
