﻿using GameSetting;
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
    protected override bool OnInteractedContinousCheck(EntityCharacterPlayer _interactor)
    {
        base.OnInteractedContinousCheck(_interactor);
        GameObjectManager.SpawnInteract<InteractPickupWeapon>( transform.position, Quaternion.identity).Play(WeaponSaveData.CreateNew(GameDataManager.m_GameWeaponUnlocked[TCommon.RandomPercentage(GameConst.D_EventWeaponVendorMachineRareRate)].RandomItem())).PlayDropAnim(transform.position+ TCommon.RandomXZCircle() * 4f);
        return true;
    }
}
