using UnityEditor;
using UnityEngine;

namespace TMKOC.StatesOfMatter
{
    [CreateAssetMenu(menuName = "ScriptableObjects/MatterSO", fileName = "MatterSOList")]
    public class MatterSO : ScriptableObject
    {
        public ItemData[] ItemList;


        public int Length { get { return ItemList.Length; } }
    }


    [System.Serializable]
    public struct ItemData
    {
        public string ItemName;
        public StateType StateType;
        public Sprite Sprite;
    }
}