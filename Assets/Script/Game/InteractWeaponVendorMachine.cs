using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class InteractWeaponVendorMachine : InteractGameBase {
    public override enum_Interaction m_InteractType => enum_Interaction.WeaponVendorMachine;
    int m_TryCount = 0;
    public new InteractWeaponVendorMachine Play()
    {
        base.Play();
        m_TradePrice = GameConst.I_EventWeaponVendorMachinePrice;
        m_TryCount = 0;
        return this;
    }
    protected override bool OnInteractedContinousCheck(EntityCharacterPlayer _interactor)
    {
        base.OnInteractedContinousCheck(_interactor);
        GameObjectManager.SpawnInteract<InteractPickupWeapon>(transform.position+TCommon.RandomXZCircle() , Quaternion.identity).Play(WeaponSaveData.New(GameDataManager.m_GameWeaponUnlocked[TCommon.RandomPercentage(GameConst.D_EventWeaponVendorMachineRate)].RandomItem()));
        m_TryCount++;
        return m_TryCount<=GameConst.I_PerkShrineTryCountMax;
    }
}
