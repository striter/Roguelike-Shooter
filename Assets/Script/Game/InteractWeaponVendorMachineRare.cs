using GameSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractWeaponVendorMachineRare : InteractGameBase
{
    public override enum_Interaction m_InteractType => enum_Interaction.WeaponVendorMachineRare;

    public new InteractWeaponVendorMachineRare Play()
    {
        base.Play();
        m_TradePrice = GameConst.I_WeaponVendorMachineRarePrice;
        return this;
    }
    protected override bool OnInteractedCheck(EntityCharacterPlayer _interactor)
    {
        base.OnInteractedCheck(_interactor);
        GameObjectManager.SpawnInteract<InteractWeaponPickup>( transform.position+ TCommon.RandomXZCircle(), Quaternion.identity).Play(WeaponSaveData.CreateNew(GameDataManager.m_WeaponRarities[TCommon.RandomPercentage(GameConst.D_EventWeaponVendorMachineRareRate)].RandomItem()));
        return true;
    }
}
