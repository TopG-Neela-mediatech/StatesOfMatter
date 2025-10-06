using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace TMKOC.StatesOfMatter
{

    public class BeakerShake : MonoBehaviour
    {
        [SerializeField] private Transform beakerTransform;
        [SerializeField] private float shakeAngle = 10f;   // how much to tilt
        [SerializeField] private float duration = 0.2f;    // speed of shake
        [SerializeField] private int vibrato = 5;          // back-and-forth oscillations

        private Tween shakeTween;

        private void Start()
        {
            ShakeBeaker();
        }

        [Button]
        public void ShakeBeaker()
        {
            if (shakeTween != null && shakeTween.IsActive()) shakeTween.Kill();

            shakeTween = beakerTransform
                .DOShakeRotation(duration, new Vector3(0, 0, shakeAngle), vibrato, 90, false)
                .OnComplete(() => beakerTransform.rotation = Quaternion.identity);
        }
    }

}