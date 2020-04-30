using System;
using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class WeaponProjectileRailgun : WeaponProjectileBase
{
    public int I_ChargeIndicatorIndex;
    public float F_ChargeDuration;
    protected int m_StoreProjectileDataIndex { get; private set; }
    SFXIndicator m_Indicator;
    
    new WeaponTriggerStoring m_Trigger;
    protected override WeaponTrigger GetTrigger()
    {
        m_Trigger= new WeaponTriggerStoring(F_ChargeDuration, OnTriggerCheck, OnStoreTriggerSuccessful);
        return m_Trigger;
    } 

    public override void OnPoolItemInit(enum_PlayerWeapon _identity, Action<enum_PlayerWeapon, MonoBehaviour> _OnRecycle)
    {
        base.OnPoolItemInit(_identity, _OnRecycle);
        m_StoreProjectileDataIndex = GameExpression.GetPlayerExtraWeaponIndex(m_WeaponInfo.m_Index);
    }

    protected void OnStoreTriggerSuccessful(bool storeScale)
    {
        OnTriggerOnce(storeScale ? m_StoreProjectileDataIndex : m_BaseSFXWeaponIndex,storeScale?GetWeaponDamageInfo(m_WeaponInfo.m_Damage):GetWeaponDamageInfo(m_WeaponInfo.m_Damage));
        OnTriggerSuccessful();
        PlayIndicator(false);
    }

    public override void Tick(bool firePausing, float fireTick, float reloadTick)
    {
        base.Tick(firePausing, fireTick*m_Attacher.m_CharacterInfo.F_Projectile_Store_TickMultiply, reloadTick);
        PlayIndicator(m_Trigger.m_Storing);
    }

    public override void OnShow(bool play)
    {
        base.OnShow(play);
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
