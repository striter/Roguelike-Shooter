using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupAmmo : PickupBase
{
    public override enum_PickupType E_Type => enum_PickupType.Ammo;
    public enum_AmmoType E_AmmoType = enum_AmmoType.Invalid;
    public int I_AmmoCount = 0;
    public float f_durationCheck { get; protected set; }
    private void Start()           
    {
        if (m_PickUpInfo == null && E_AmmoType !=  enum_AmmoType.Invalid) //Init While Word Drop
        {
            m_PickUpInfo = new PickupInfoAmmo(E_AmmoType,I_AmmoCount);
        }
    }
}
public class PickupInfoAmmo : PickupInfoBase
{
    public enum_AmmoType E_Type { get; private set; }
    public int I_AmmoCount { get; private set; }
    public PickupInfoAmmo(enum_AmmoType type,int count):base(type.ToPickup(count))
    {
        E_Type = type;
        I_AmmoCount = count;
    }
}
