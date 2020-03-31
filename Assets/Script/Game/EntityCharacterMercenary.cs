using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
using System;

public class EntityCharacterMercenary : EntityCharacterBase {
    public enum_MercenaryCharacter m_Character;
    public WeaponBase m_Weapon { get; private set; }
    public override MeshRenderer m_WeaponSkin => m_Weapon.m_WeaponSkin;

    public override void OnPoolItemInit(int _identity, Action<int, MonoBehaviour> _OnRecycle)
    {
        base.OnPoolItemInit(_identity, _OnRecycle);
    }

    public void OnMercenaryActivate(enum_EntityFlag flag, MercenarySaveData mercenaryData)
    {
        base.OnMainCharacterActivate(flag);
        m_Weapon = GameObjectManager.SpawnWeapon(mercenaryData.m_Weapon);
    }

    protected override void OnAliveTick(float deltaTime)
    {
        base.OnAliveTick(deltaTime);
    }


}
