using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class WeaponProjectileRailgun : WeaponProjectileBase {
    public int I_ChargeIndicatorIndex;
    SFXIndicator m_Indicator;
    protected override WeaponTrigger GetTrigger() => new WeaponTriggerStoring(m_WeaponInfo.m_FireRate, OnTriggerCheck, OnStoreTriggerSuccessful);

    protected void OnStoreTriggerSuccessful(float storeScale)
    {
        FireProjectiles(storeScale);
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
            m_Indicator.PlayLoop(m_Attacher.m_EntityID);
        }

        if(!play&&m_Indicator)
        {
            m_Indicator.Stop();
            m_Indicator = null;
        }
    }
}
