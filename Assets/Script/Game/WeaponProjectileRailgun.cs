using System;
using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class WeaponProjectileRailgun : WeaponProjectileBase {
    public int I_ChargeIndicatorIndex;
    protected int m_StoreProjectileDataIndex { get; private set; }
    SFXIndicator m_Indicator;
    public override float F_BaseDamage => GameObjectManager.GetSFXWeaponData<SFXProjectile>(m_StoreProjectileDataIndex).F_Damage;
    protected override WeaponTrigger GetTrigger() => new WeaponTriggerStoring(m_WeaponInfo.m_FireRate, OnTriggerCheck, OnStoreTriggerSuccessful);

    public override void OnPoolItemInit(enum_PlayerWeapon _identity, Action<enum_PlayerWeapon, MonoBehaviour> _OnRecycle)
    {
        base.OnPoolItemInit(_identity, _OnRecycle);
        m_StoreProjectileDataIndex = GameExpression.GetPlayerExtraWeaponIndex(m_WeaponInfo.m_Index);
    }

    protected void OnStoreTriggerSuccessful(float storeScale)
    {
        FireProjectiles(storeScale == 1? m_StoreProjectileDataIndex : m_BaseSFXWeaponIndex);
        OnTriggerSuccessful();
        PlayIndicator(false);
    }
    public override void Trigger(bool down)
    {
        base.Trigger(down);
        if(down&&OnTriggerCheck())
            PlayIndicator(true);
        if(!down)
            PlayIndicator(false);
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
