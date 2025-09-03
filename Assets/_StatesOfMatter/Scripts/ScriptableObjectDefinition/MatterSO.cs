using UnityEditor;
using UnityEngine;

namespace TMKOC.StatesOfMatter
{
    [CreateAssetMenu(menuName = "ScriptableObjects/MatterSO", fileName = "MatterSOList")]
    public class MatterSO : ScriptableObject
    {
        public ItemData[] ItemList;

        public int Length { get { return ItemList.Length; } }

        public ItemData GetItem(int index)
        {
            if(index < 0 || index >= ItemList.Length)
            {
                return null;
            }

            return ItemList[index];
        }
    }


    [System.Serializable]
    public class ItemData
    {
        public string ItemName;
        public StateType StateType;
        public Sprite Sprite;
    }
}