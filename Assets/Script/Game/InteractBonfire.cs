using System;
using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class InteractBonfire : InteractGameBase {
    public override enum_Interaction m_InteractType => enum_Interaction.Bonfire;
    TSpecialClasses.ParticleControlBase m_FireParticles;
    Transform tf_Light;
    DamageInfo m_HealInfo;
    TimeCounter m_DistinguishCheck=new TimeCounter(5f);
    public override void OnPoolItemInit(enum_Interaction identity, Action<enum_Interaction, MonoBehaviour> OnRecycle)
    {
        base.OnPoolItemInit(identity, OnRecycle);
        m_FireParticles = new TSpecialClasses.ParticleControlBase(transform.Find("Fire"));
        tf_Light = transform.Find("Light");
        m_HealInfo = new DamageInfo(0, enum_DamageType.HealthOnly, DamageDeliverInfo.BuffInfo(-1, SBuff.m_PlayerBoneFireHealBuff));
    }
    public new void Play()
    {
        base.Play();
        OnDistinguish();
    }

    protected override bool OnInteractOnceCanKeepInteract(EntityCharacterPlayer _interactor)
    {
        base.OnInteractOnceCanKeepInteract(_interactor);
        m_DistinguishCheck.Reset();
        _interactor.m_HitCheck.TryHit(m_HealInfo);

        m_FireParticles.Play();
        tf_Light.SetActivate(true);
        return false;
    }

    void OnDistinguish()        //Light Low,Fire Off,Start Distinguish
    {
        m_FireParticles.Stop();
        tf_Light.SetActivate(false);
    }

    private void Update()
    {
        if (!m_DistinguishCheck.m_Timing)
            return;
        m_DistinguishCheck.Tick(Time.deltaTime);
        if (!m_DistinguishCheck.m_Timing)
            OnDistinguish();
    }
}
