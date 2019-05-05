using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectorPickup : DetectorBase<PickupBase>
{
    Func<PickupBase, bool> OnPickupDetected;
    protected override void Awake()
    {
        base.Awake();
        this.gameObject.layer = GameLayers.IL_DynamicDetector;
    }
    public void Init(Func<PickupBase,bool> _OnPickupDetected)
    {
        OnPickupDetected = _OnPickupDetected;
    }
    protected override PickupBase GetDetectTarget(Collider other)
    {
        return other.tag == GameTags.CT_Pickup?other.GetComponent<PickupBase>():null;
    }
    public void CheckPickups()
    {
        OnDetectChanged();
    }
    protected override void OnDetectChanged()
    {
        base.OnDetectChanged();
        for (int i = 0; i < l_targets.Count; i++)
        {
            if (!l_targets[i].b_pickable || OnPickupDetected(l_targets[i]))
            {
                l_targets.Remove(l_targets[i]);
            }
        }
    }
}
