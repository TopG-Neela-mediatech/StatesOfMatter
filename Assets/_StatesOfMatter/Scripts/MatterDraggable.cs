using UnityEngine;

public class MatterDraggable : AdvancedDraggable
{
    public Category correctCategory;

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
