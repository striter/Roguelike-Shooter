using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class InteractWeaponRecycle : InteractGameBase {
    public override enum_Interaction m_InteractType => enum_Interaction.WeaponRecycle;
    public new InteractWeaponRecycle Play()
    {
        base.Play();
        return this;
    }

    protected override bool OnTryInteractCheck(EntityCharacterPlayer _interactor)=> base.OnTryInteractCheck(_interactor)&& _interactor.m_Weapon1 && _interactor.m_Weapon2;
    protected override bool OnInteractedCheck(EntityCharacterPlayer _interactor)
    {
        base.OnInteractedCheck(_interactor);
        WeaponBase weapon = _interactor.RecycleWeapon();
        weapon.DoItemRecycle();
        _interactor.m_CharacterInfo.OnCoinsGain(GameConst.D_EventWeaponRecyclePrice[weapon.m_WeaponInfo.m_Rarity]);
        return false;
    }
}
