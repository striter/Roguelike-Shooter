using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class InteractSafeCrack : InteractGameBase {
    public override enum_Interaction m_InteractType => enum_Interaction.SafeCrack;
    public new InteractSafeCrack Play()
    {
        base.Play();
        m_TradePrice = GameConst.I_EventSafeCrackPrice;
        return this;
    }

    protected override bool OnInteractedContinousCheck(EntityCharacterPlayer _interactor)
    {
        base.OnInteractedContinousCheck(_interactor);

        for(int i=0;i<GameConst.I_EventSafeCoinsCount;i++)
        GameObjectManager.SpawnInteract<InteractPickupCoin>(transform.position , Quaternion.identity).Play(GameConst.I_EventSafeCoinsAmount).PlayDropAnim(transform.position+ TCommon.RandomXZCircle());

        List<int> perks = GameDataManager.RandomPerks(GameConst.RI_EventSafePerkCount.Random(),GameConst.D_EventSafePerkRate,_interactor.m_CharacterInfo.m_ExpirePerks);
        for (int i = 0; i < perks.Count; i++)
            GameObjectManager.SpawnInteract<InteractPerkPickup>(transform.position + TCommon.RandomXZCircle(), Quaternion.identity).Play(perks[i]);

        int weaponCount = GameConst.RI_EventSafeWeaponCount.Random();
        for (int i = 0; i < weaponCount; i++)
            GameObjectManager.SpawnInteract<InteractPickupWeapon>(transform.position + TCommon.RandomXZCircle(), Quaternion.identity).Play(WeaponSaveData.New(GameDataManager.m_GameWeaponUnlocked[TCommon.RandomPercentage(GameConst.D_EventSafeWeaponRate)].RandomItem()));
        return false;
    }

}
