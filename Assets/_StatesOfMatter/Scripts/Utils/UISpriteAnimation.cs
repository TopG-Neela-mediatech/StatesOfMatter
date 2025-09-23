using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
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
        [SerializeField] protected bool isPlaying;
        [SerializeField] protected bool playOnAwake;

        public bool IsPlaying { get => isPlaying; set => isPlaying = value; }
        protected Coroutine coroutineAnim;
        private bool toggleFlag = false;

        private void Awake()
        {
            StopAnimCoroutine();
        }

        private void OnEnable()
        {
            if(playOnAwake)
            {
                PlayUIAnim();
                toggleFlag = !toggleFlag;
            }
        }

        [Button]
        public virtual void PlayUIAnim()
        {
            if (isPlaying) return;
            coroutineAnim = StartCoroutine(PlayAnimUI_Coroutine());
        }

        public virtual void StopUIAnim()
        {
            StopCoroutine(PlayAnimUI_Coroutine());
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!toggleFlag) PlayUIAnim();
            else StopAnimCoroutine();
            toggleFlag = !toggleFlag;
        }

        public virtual IEnumerator PlayAnimUI_Coroutine()
        {
            IsPlaying = true;
            yield return new WaitForSeconds(speed);
            if (indexSprite >= spriteArray.Length)  //end of animation...
            {
                IsPlaying = false;
                indexSprite = 0;
                // yield return new WaitForSeconds(2);

                image.sprite = spriteArray[indexSprite];   //reset to first frame...    
                if (!loop)
                {
                    // Debug.Log("anim end...");
                    yield break;   //run only once...
                }
            }
            image.sprite = spriteArray[indexSprite];

            indexSprite += 1;
            coroutineAnim = StartCoroutine(PlayAnimUI_Coroutine());
        }

        [Button]
        public virtual void StopAnimCoroutine()
        {
            if (coroutineAnim != null)
                StopCoroutine(coroutineAnim);
            coroutineAnim = null;
            indexSprite = 0;
            image.sprite = spriteArray[0];
            isPlaying = false;
        }
    }



}
