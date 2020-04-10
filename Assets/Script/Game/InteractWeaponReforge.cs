using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class InteractWeaponReforge : InteractGameBase
{
    public override enum_Interaction m_InteractType => enum_Interaction.WeaponReforge;
    int m_ReforgeTime;
    public new InteractWeaponReforge Play()
    {
        base.Play();
        m_ReforgeTime = 0;
        return this;
    }
    protected override bool OnInteractedContinousCheck(EntityCharacterPlayer _interactor)
    {
        base.OnInteractedContinousCheck(_interactor);
        _interactor.ReforgeWeapon(GameObjectManager.SpawnWeapon(WeaponSaveData.CreateNew(GameDataManager.m_GameWeaponUnlocked[TCommon.RandomPercentage(GameConst.D_EventWeaponReforgeRate, null)].RandomItem())));
        m_ReforgeTime++;
        if (m_ReforgeTime == 1)
            m_TradePrice = GameConst.I_EventWeaponReforgeSecondPrice;
        return m_ReforgeTime > 1;
    }
}
