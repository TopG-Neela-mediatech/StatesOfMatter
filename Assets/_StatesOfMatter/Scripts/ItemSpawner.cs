using UnityEngine;

namespace TMKOC.StatesOfMatter
{
    public class ItemSpawner : MonoBehaviour
    {
        [SerializeField] private MatterDraggable itemPrefab;
        [SerializeField] private Transform spawnParent, secondarySpawnParent;

        private MatterDraggable currentSpawnedItem;

        private void OnEnable()
        {
            GameManager.OnRequestNextItem += SpawnNextItem;
        }

        private void OnDisable()
        {
            GameManager.OnRequestNextItem -= SpawnNextItem;
        }

        public void SpawnNextItem(ItemData data)
        {
            currentSpawnedItem = Instantiate(itemPrefab, spawnParent);

            currentSpawnedItem.SetResetType(false);
            currentSpawnedItem.SetInfo(data);
            currentSpawnedItem.ToggleTextObj(false);
            currentSpawnedItem.SetNewParent(secondarySpawnParent);
        }

    }
}