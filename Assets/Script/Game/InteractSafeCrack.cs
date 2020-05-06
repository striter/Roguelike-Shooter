﻿using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class InteractSafeCrack : InteractGameBase {
    public override enum_Interaction m_InteractType => enum_Interaction.SafeCrack;
    public new InteractSafeCrack Play()
    {
        base.Play();
        m_KeyRequire =1;
        return this;
    }

    protected override bool OnInteractedContinousCheck(EntityCharacterPlayer _interactor)
    {
        base.OnInteractedContinousCheck(_interactor);

        int coinsAmount = GameConst.I_EventSafeCoinsAmount.Random();
        for(int i=0;i< coinsAmount; i++)
        GameObjectManager.SpawnInteract<InteractPickupCoin>(transform.position , Quaternion.identity).Play(1).PlayDropAnim(transform.position+ TCommon.RandomXZCircle()).PlayMoveAnim(_interactor.transform.position);

        List<int> perks = GameDataManager.RandomPlayerPerks(GameConst.RI_EventSafePerkCount.Random(),GameConst.D_EventSafePerkRate,_interactor.m_CharacterInfo.m_ExpirePerks);
        for (int i = 0; i < perks.Count; i++)
            GameObjectManager.SpawnInteract<InteractPerkPickup>(transform.position + TCommon.RandomXZCircle(), Quaternion.identity).Play(perks[i]);

        int weaponCount = GameConst.RI_EventSafeWeaponCount.Random();
        for (int i = 0; i < weaponCount; i++)
            GameObjectManager.SpawnInteract<InteractPickupWeapon>(transform.position + TCommon.RandomXZCircle(), Quaternion.identity).Play(WeaponSaveData.New(GameDataManager.m_GameWeaponUnlocked[TCommon.RandomPercentage(GameConst.D_EventSafeWeaponRate)].RandomItem()));
        return false;
    }

}
