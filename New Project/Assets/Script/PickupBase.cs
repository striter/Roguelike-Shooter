using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PickupBase : MonoBehaviour
{
    public virtual enum_PickupType E_Type => enum_PickupType.Invalid;
    public bool b_pickable { get; protected set; }
    public HitCheckBase m_hitCheck { get; private set; }
    public PickupInfoBase m_PickUpInfo;
    protected virtual void Awake()
    {
        b_pickable = true;
        m_hitCheck = GetComponent<HitCheckBase>();
        this.gameObject.tag = GameTags.CT_Pickup;
        this.transform.SetParent(EntityManager.tf_PickupRoot);
    }
    public PickupInfoBase PickUp()
    {
        EntityManager.RecyclePickup(m_PickUpInfo.E_PickupType,this);
        return m_PickUpInfo;
    }
}
public class PickupInfoBase
{
    public enum_Pickup E_PickupType { get; private set; } = enum_Pickup.Invalid;
    public PickupInfoBase(enum_Pickup pickupType)
    {
        E_PickupType = pickupType;
    }
}