using UnityEngine;

namespace TMKOC.StatesOfMatter
{
    public class ItemSpawner : MonoBehaviour
    {
        [SerializeField] private MatterDraggable itemPrefab;
        [SerializeField] private Transform spawnParent;
       
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
            MatterDraggable newItem = Instantiate(itemPrefab, spawnParent);

            newItem.SetInfo(data);
        }
    }
}