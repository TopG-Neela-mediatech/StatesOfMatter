using TMKOC.StatesOfMatter;
using TMPro;
using UnityEngine;

public class MatterDraggable : AdvancedDraggable
{
    [Space]
    [Header("Item Info")]
    [SerializeField] private string ItemName;
    [SerializeField] private StateType correctCategory;
    [SerializeField] private UnityEngine.UI.Image image;
    [SerializeField] private TextMeshProUGUI textBox;

    public void SetInfo(ItemData itemData)
    {
        textBox.SetText(itemData.ItemName);
        correctCategory = itemData.StateType;
        if (itemData.Sprite)
            image.sprite = itemData.Sprite;
    }

    protected override bool IsValidDropTarget(GameObject target)
    {
        if (target == null)
        {
            return false;
        }

        DropZone zone = target.GetComponent<DropZone>() ?? target.GetComponentInParent<DropZone>();
        if (zone == null)
        {
            Debug.Log($"IsValidDropTarget: {target.name} has NO DropZone component");
            return false;
        }

        if (zone.zoneCategory == correctCategory)
        {
            bool placed = zone.PlaceDraggable(this);
            return placed;
        }

        return false;
    }

}
