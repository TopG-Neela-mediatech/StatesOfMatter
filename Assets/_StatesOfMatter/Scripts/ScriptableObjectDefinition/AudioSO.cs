using UnityEngine;

namespace TMKOC.StatesOfMatter
{
    [System.Serializable]
    public class AudioData
    {
        public TutorialAudio AudioType;
        public AudioClip[] Clip;
    }

    [CreateAssetMenu(fileName = "AudioSO", menuName = "TMKOC/AudioSO")]
    public class AudioSO : ScriptableObject
    {
        [Header("List of audio clips by type")]
        public AudioData[] AudioList;

        /// <summary>
        /// Returns a random AudioClip for the given TutorialAudio type.
        /// Returns null if no clips are found.
        /// </summary>
        public AudioClip GetRandomAudioClip(TutorialAudio audioType)
        {
            var data = GetAudioData(audioType);
            if (data == null || data.Clip.Length == 0) return null;

            return data.Clip[UnityEngine.Random.Range(0, data.Clip.Length)];
        }

        /// <summary>
        /// Returns a specific AudioClip for the given TutorialAudio type and index.
        /// Returns null if the index is invalid or clips do not exist.
        /// </summary>
        public AudioClip GetSpecificAudioClip(TutorialAudio audioType, int index)
        {
            var data = GetAudioData(audioType);
            if (data == null || index < 0 || index >= data.Clip.Length) return null;

            return data.Clip[index];
        }

        /// <summary>
        /// Checks if the provided index is valid for SlideSpecific audio.
        /// Returns false if clips are missing or index is out of bounds.
        /// </summary>
        public bool IsValidIndex(int index)
        {
            AudioData audioData = GetAudioData(TutorialAudio.SlideSpecific);
            if (audioData == null || audioData.Clip.Length == 0) return false;

            return index >= 0 && index < audioData.Clip.Length;
        }

        /// <summary>
        /// Returns the AudioData for the given TutorialAudio type.
        /// Logs a warning if not found.
        /// </summary>
        private AudioData GetAudioData(TutorialAudio audioType)
        {
            for (int i = 0; i < AudioList.Length; i++)
            {
                if (AudioList[i].AudioType == audioType)
                    return AudioList[i];
            }

            Debug.LogWarning($"AudioData not found for {audioType} in {name}");
            return null;
        }
    }
}
