using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class InteractWeaponVendorMachine : InteractGameBase {
    public override enum_Interaction m_InteractType => enum_Interaction.WeaponVendorMachine;
    int m_TryCount = 0;
    public InteractWeaponVendorMachine Play(float tradePrice)
    {
        base.Play();
        SetTradePrice(tradePrice);
        m_TryCount = 0;
        return this;
    }
    protected override bool OnInteractedContinousCheck(EntityCharacterPlayer _interactor)
    {
        base.OnInteractedContinousCheck(_interactor);
        GameManager.Instance.SpawnRandomUnlockedWeapon(transform.position + TCommon.RandomXZCircle(), Quaternion.identity,GameConst.D_EventWeaponVendorMachineRate);
        m_TryCount++;
        return m_TryCount<=GameConst.I_EventWeaponVendorTryCount;
    }
}
