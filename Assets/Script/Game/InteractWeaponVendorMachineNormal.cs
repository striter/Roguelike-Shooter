using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class InteractWeaponVendorMachineNormal : InteractGameBase {
    public override enum_Interaction m_InteractType => enum_Interaction.WeaponVendorMachineNormal;

    public new InteractWeaponVendorMachineNormal Play()
    {
        base.Play();
        m_TradePrice = GameConst.I_WeaponVendorMachineNormalPrice;
        return this;
    }
    protected override bool OnInteractedCheck(EntityCharacterPlayer _interactor)
    {
        base.OnInteractedCheck(_interactor);
        GameObjectManager.SpawnInteract<InteractWeaponPickup>(transform.position+TCommon.RandomXZCircle() , Quaternion.identity).Play(WeaponSaveData.CreateNew(GameDataManager.m_GameWeaponUnlocked[TCommon.RandomPercentage(GameConst.D_EventWeaponVendorMachineNormalRate)].RandomItem()));
        return true;
    }
}
