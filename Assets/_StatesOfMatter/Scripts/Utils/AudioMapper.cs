using UnityEngine;
using System;
using System.Reflection;

namespace TMKOC.StatesOfMatter
{
    public class AudioMapper : GenericSingleton<AudioMapper>
    {
        [Header("Tutorial")]
        public string GameStart = "GameStart";
        public string GameEnd = "GameEnd";

        [Header("Gameplay")]
        public string[] Slides = { "Slide1", "Slide2", "Slide3", "Slide4", "Slide5", "Slide6" };

        #region Core Functionality

        /// <summary>
        /// Retrieves a string key from a named array field using reflection.
        /// </summary>
        /// <param name="arrayName">The name of the string array field.</param>
        /// <param name="index">The index within the array.</param>
        public string GetKeyByIndex(string arrayName, int index)
        {
            FieldInfo field = GetType().GetField(arrayName, BindingFlags.Public | BindingFlags.Instance);
            if (field != null && field.FieldType == typeof(string[]))
            {
                string[] array = (string[])field.GetValue(this);
                if (array != null && index >= 0 && index < array.Length)
                {
                    return array[index];
                }
            }
            Debug.LogWarning($"AudioMapper: Array '{arrayName}' not found or index {index} out of bounds.");
            return string.Empty;
        }

        /// <summary>
        /// Enum bridge for TutorialAudio compatibility.
        /// Returns a string key if applicable, or triggers specific RuntimeAudioLoader calls for feedback.
        /// </summary>
        public string GetRandomKeyFor(Enum audioType)
        {
            if (audioType is TutorialAudio type)
            {
                switch (type)
                {
                    case TutorialAudio.OnGameStart:
                        return GameStart;
                    case TutorialAudio.OnGameStop:
                        return GameEnd;
                    case TutorialAudio.Correct:
                        RuntimeAudioLoader.Instance.PlayCorrectAudioClip();
                        return "HandledViaCommonAudio"; // Special flag to indicate internal handling
                    case TutorialAudio.Incorrect:
                        RuntimeAudioLoader.Instance.PlayIncorrectAudioClip();
                        return "HandledViaCommonAudio";
                    case TutorialAudio.Retry:
                        RuntimeAudioLoader.Instance.PlayRetryAudioClip();
                        return "HandledViaCommonAudio";
                    default:
                        return string.Empty;
                }
            }
            return string.Empty;
        }

        #endregion
    }
}