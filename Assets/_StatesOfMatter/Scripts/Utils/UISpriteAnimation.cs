using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TMKOC.StatesOfMatter
{
    public class UISpriteAnimation : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] protected Image image;
        [SerializeField] protected Sprite[] spriteArray;
        [SerializeField] protected float speed = .02f;
        [SerializeField] protected int indexSprite;
        [SerializeField] protected bool loop;
        [SerializeField] protected float loopDelay = 1.25f;
        [SerializeField] protected bool isPlaying;
        [SerializeField] protected bool playOnAwake;
        [SerializeField] protected float initialDelay;
        [SerializeField] protected bool canPause;

        private bool isPaused = false;
        private bool toggleFlag = false;
        protected Coroutine coroutineAnim;

        // Optional: Events for external systems to hook into
        [FoldoutGroup("Events")] public UnityEvent onAnimationComplete;
        [FoldoutGroup("Events")] public UnityEvent onAnimationLoop;

        public bool IsPlaying { get => isPlaying; set => isPlaying = value; }
        public bool IsPaused => isPaused;

        private void Awake()
        {
            // Validate references
            if (image == null)
            {
                Debug.LogError($"Image reference is missing on {gameObject.name}", this);
                enabled = false;
                return;
            }

            if (spriteArray == null || spriteArray.Length == 0)
            {
                Debug.LogError($"Sprite array is empty on {gameObject.name}", this);
                enabled = false;
                return;
            }

            StopUIAnim();
        }

        private void OnEnable()
        {
            if (playOnAwake && !isPlaying)
            {
                PlayUIAnim();
            }
        }

        private void OnDisable()
        {
            // Clean up coroutine when disabled
            if (coroutineAnim != null)
            {
                StopCoroutine(coroutineAnim);
                coroutineAnim = null;
            }
        }

        public void PlayUIAnim()
        {
            // Already playing and not paused - do nothing
            if (isPlaying && !isPaused)
                return;

            if (isPlaying && isPaused)
            {
                // Resume from pause
                isPaused = false;
            }
            else
            {
                // Start playing new animation
                isPlaying = true;
                isPaused = false;

                // Stop any existing coroutine before starting new one
                if (coroutineAnim != null)
                    StopCoroutine(coroutineAnim);

                coroutineAnim = StartCoroutine(PlayAnimUI_Coroutine());
            }
        }

        public void PauseUIAnim()
        {
            if (isPlaying && !isPaused)
            {
                isPaused = true;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!canPause) return;

            if (!toggleFlag)
                PlayUIAnim();
            else
                PauseUIAnim();

            toggleFlag = !toggleFlag;
        }

        private IEnumerator PlayAnimUI_Coroutine()
        {
            // Apply initial delay only on first start
            if (initialDelay > 0)
                yield return new WaitForSeconds(initialDelay);

            while (isPlaying)
            {
                // Wait while paused
                while (isPaused)
                    yield return null;

                // Check if we've reached the end
                if (indexSprite >= spriteArray.Length)
                {
                    if (!loop)
                    {
                        isPlaying = false;
                        onAnimationComplete?.Invoke();
                        yield break;
                    }

                    // Loop back to start
                    indexSprite = 0;
                    onAnimationLoop?.Invoke();

                    if (loopDelay > 0)
                        yield return new WaitForSeconds(loopDelay);
                }

                // Update sprite
                image.sprite = spriteArray[indexSprite];
                indexSprite++;

                yield return new WaitForSeconds(speed);
            }
        }

        public void StopUIAnim()
        {
            if (coroutineAnim != null)
            {
                StopCoroutine(coroutineAnim);
                coroutineAnim = null;
            }

            isPlaying = false;
            isPaused = false;
            indexSprite = 0;

            // Safely set to first sprite
            if (spriteArray != null && spriteArray.Length > 0 && image != null)
                image.sprite = spriteArray[0];
        }

        // Utility method to restart animation from beginning
        [Button("Restart Animation")]
        public void RestartUIAnim()
        {
            StopUIAnim();
            PlayUIAnim();
        }

        // Utility method to set animation speed at runtime
        public void SetSpeed(float newSpeed)
        {
            speed = Mathf.Max(0.001f, newSpeed); // Prevent negative or zero speed
        }
    }
}