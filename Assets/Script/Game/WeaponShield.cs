using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class WeaponShield : WeaponBase {
    public override enum_PlayerWeaponBaseType m_WeaponType => enum_PlayerWeaponBaseType.Shield;

    public int I_IndicatorIndex;
    SFXIndicator m_Indicator;

    protected override void OnAutoTrigger(float animDuration)
    {
        OnAmmoCost();
    }

    public override void OnShow(bool showing)
    {
        base.OnShow(showing);
        if (!showing)
            PlayIndicator(false);
    }

    public override void WeaponTick(bool firePausing, float deltaTime)
    {
        base.WeaponTick(firePausing, deltaTime);
        PlayIndicator(m_Trigger.m_Triggering);
    }

    public override bool OnDamageBlockCheck(DamageInfo info) {
        bool success = m_Trigger.m_Triggering;
        if (success)
            OnAttackAnim(1f);
        return success;
    }


    void PlayIndicator(bool play)
    {
        if (play && !m_Indicator)
        {
            m_Indicator = GameObjectManager.SpawnIndicator(I_IndicatorIndex, m_Attacher.tf_Head.position, Vector3.up);
            m_Indicator.AttachTo(m_Attacher.tf_Head);
            m_Indicator.PlayControlled(m_Attacher.m_EntityID);
            m_Indicator.transform.localScale = Vector3.one;
        }

        if (!play && m_Indicator)
        {
            m_Indicator.Stop();
            m_Indicator = null;
        }
    }
}
