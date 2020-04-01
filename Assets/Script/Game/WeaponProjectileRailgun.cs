﻿using System;
using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class WeaponProjectileRailgun : WeaponProjectileBase
{
    public float F_StoreDamage;
    public int I_ChargeIndicatorIndex;
    protected int m_StoreProjectileDataIndex { get; private set; }
    SFXIndicator m_Indicator;
    
    new WeaponTriggerStoring m_Trigger;
    protected override WeaponTrigger GetTrigger()
    {
        m_Trigger= new WeaponTriggerStoring(m_WeaponInfo.m_FireRate, OnTriggerCheck, OnStoreTriggerSuccessful);
        return m_Trigger;
    } 

    public override void OnPoolItemInit(enum_PlayerWeapon _identity, Action<enum_PlayerWeapon, MonoBehaviour> _OnRecycle)
    {
        base.OnPoolItemInit(_identity, _OnRecycle);
        m_StoreProjectileDataIndex = GameExpression.GetPlayerExtraWeaponIndex(m_WeaponInfo.m_Index);
    }

    protected void OnStoreTriggerSuccessful(bool storeScale)
    {
        FireProjectiles(storeScale ? m_StoreProjectileDataIndex : m_BaseSFXWeaponIndex,storeScale?GetWeaponDamageInfo(F_StoreDamage):GetWeaponDamageInfo(F_BaseDamage));
        OnTriggerSuccessful();
        PlayIndicator(false);
    }

    public override void Tick(bool firePausing, float fireTick, float reloadTick)
    {
        base.Tick(firePausing, fireTick, reloadTick);
        PlayIndicator(m_Trigger.m_Storing);
    }

    public override void OnPlay(bool play)
    {
        base.OnPlay(play);
        if (!play)
            PlayIndicator(false);
    }

    void PlayIndicator(bool play)
    {
        if(play&&!m_Indicator)
        {
            m_Indicator = GameObjectManager.SpawnIndicator(I_ChargeIndicatorIndex, m_Muzzle.position, m_Muzzle.forward);
            m_Indicator.AttachTo(m_Muzzle);
            m_Indicator.PlayControlled(m_Attacher.m_EntityID);
        }

        if(!play&&m_Indicator)
        {
            m_Indicator.Stop();
            m_Indicator = null;
        }
    }
}
