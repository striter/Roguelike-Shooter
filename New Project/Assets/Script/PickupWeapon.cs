using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupWeapon : PickupBase
{
    public override enum_PickupType E_Type => enum_PickupType.Weapon;
    public enum_WeaponType E_WeaponType = enum_WeaponType.Invalid;
    public int I_ClipAmmo=0;
    public float f_durationCheck { get; protected set; }
    private void Start()
    {
        if (m_PickUpInfo == null && E_WeaponType != enum_WeaponType.Invalid)
        {
            if (E_WeaponType == enum_WeaponType.Shotgun)
            {
                m_PickUpInfo = new PickupInfoWeaponShotgun(I_ClipAmmo, false);
            }
            else
                m_PickUpInfo = new PickupInfoWeapon(I_ClipAmmo,E_WeaponType);
        }
    }
    public void Throw(float strength,Vector3 direction,float pickUpDuration)
    {
        (m_hitCheck as HitCheckDynamic).Throw(strength,direction);
        this.transform.rotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
        f_durationCheck = Time.time + pickUpDuration;
        b_pickable = false;
    }

    private void Update()
    {
        if (!b_pickable&&Time.time > f_durationCheck)
        {
            b_pickable = true;
        }
    }
}

public class PickupInfoWeapon : PickupInfoBase
{
    public int I_ClipAmmo { get; private set; }
    public enum_WeaponType E_WeaponType { get; private set; }
    public PickupInfoWeapon(WeaponBase wb) : base(wb.E_WeaponType.ToPickup())
    {
        I_ClipAmmo = wb.I_AmmoLeft;
        E_WeaponType = wb.E_WeaponType;
    }
    public PickupInfoWeapon(int clipAmmo, enum_WeaponType type):base(type.ToPickup())
    {
        I_ClipAmmo = clipAmmo;
        E_WeaponType = type;
    }
}

public class PickupInfoWeaponShotgun : PickupInfoWeapon
{
    public bool B_Pump { get; private set; }
    public PickupInfoWeaponShotgun(WeaponPlayerShotgun wb) : base(wb)
    {
        B_Pump = wb.b_needPump;
    }
    public PickupInfoWeaponShotgun(int clipAmmo,bool pump):base(clipAmmo, enum_WeaponType.Shotgun)
    {
        pump = false;
    }
}
