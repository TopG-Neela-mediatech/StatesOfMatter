using UnityEngine;
using System;

namespace TMKOC.StatesOfMatter
{
    public class GameManager : GenericSingleton<GameManager>
    {
        [SerializeField] private MatterSO matterSO; 

        private int currentIndex = 0;

        #region Game events

        public static event Action OnGameStart;
        public static event Action<ItemData> OnRequestNextItem;
        public static event Action OnCorrectDrop, OnIncorrectDrop;

        #endregion

        private void Start()
        {
            OnGameStart?.Invoke();
        }

        public void RequestRandomItem()
        {
            var randomIndex = UnityEngine.Random.Range(0, matterSO.Length);

            var item = matterSO.ItemList[randomIndex];
            OnRequestNextItem?.Invoke(item);
        }

        public void RequestNextItem()
        {
            if (currentIndex >= matterSO.Length)
            {
                Debug.Log("All items spawned!");
                return;
            }

            var item = matterSO.GetItem(currentIndex);

            OnRequestNextItem?.Invoke(item);

            currentIndex++;
        }

        // This will be called when a correct drag happens
        public void OnCorrectDrag()
        {
            Debug.Log("Correct Drag detected! Requesting next item...");
            OnCorrectDrop?.Invoke();  
        }
        public void OnIncorrectDrag()
        {
            Debug.Log("Incorrect Drag detected! Requesting next item...");
            OnIncorrectDrop?.Invoke();
        }
    }
}