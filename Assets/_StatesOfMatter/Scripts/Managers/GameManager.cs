using UnityEngine;
using System;

namespace TMKOC.StatesOfMatter
{
    public class GameManager : GenericSingleton<GameManager>
    {
        [SerializeField] private MatterSO matterSO;

        private int currentIndex = 0;

        public static event Action<ItemData> OnRequestNextItem;

        private void Start()
        {
            RequestNextItem();
        }

        public void RequestNextItem()
        {
            if (currentIndex >= matterSO.Length)
            {
                Debug.Log("All items spawned!");
                return;
            }

            var item = matterSO.ItemList[currentIndex];

            OnRequestNextItem?.Invoke(item);

            currentIndex++;
        }   

        // This will be called when a correct drag happens
        public void OnCorrectDrag()
        {
            Debug.Log("Correct Drag detected! Requesting next item...");
            RequestNextItem();
        }
    }
}