using UnityEngine;

public class InteractPickup : InteractGameBase
{
    public override bool B_InteractOnTrigger => true;
    protected override bool B_RecycleOnInteract => true;
}
