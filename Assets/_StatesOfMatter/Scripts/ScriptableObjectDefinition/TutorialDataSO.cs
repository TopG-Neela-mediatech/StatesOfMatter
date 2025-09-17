using UnityEngine;

namespace TMKOC.StatesOfMatter
{
    [CreateAssetMenu(menuName = "ScriptableObjects/TutorialSO", fileName = "TutorialDataList")]
    public class TutorialDataSO : ScriptableObject
    {
        public TutorialData[] TutorialList;

        public int Length { get { return TutorialList.Length; } }

        public TutorialData GetData(int index)
        {
            if (index < 0 || index >= TutorialList.Length)
            {
                return null;
            }

            return TutorialList[index];
        }
    }

    [System.Serializable]
    public class TutorialData
    {
        public int Index;
        public string Heading;
        [TextArea(3, 6)]
        public string PrimaryText1;
        [TextArea(3, 5)]
        public string PrimaryText2;
        [TextArea(3, 5)]
        public string SecondaryText;
        public Sprite Sprite;
    }
}