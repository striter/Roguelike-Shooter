using System;
using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class InteractBonfire : InteractGameBase {
    public override enum_Interaction m_InteractType => enum_Interaction.Bonfire;
    protected override bool B_CanInteract(EntityCharacterPlayer _interactor) => false;
    TSpecialClasses.ParticleControlBase m_FireParticles;
    Transform tf_Light;
    EntityCharacterPlayer m_Target;
    DamageInfo m_HealInfo;
    float m_distinguishCheck, m_healBuffCheck;
    public override void OnPoolItemInit(enum_Interaction identity, Action<enum_Interaction, MonoBehaviour> OnRecycle)
    {
        base.OnPoolItemInit(identity, OnRecycle);
        m_FireParticles = new TSpecialClasses.ParticleControlBase(transform.Find("Fire"));
        tf_Light = transform.Find("Light");
        m_HealInfo = new DamageInfo(0, enum_DamageType.HealthOnly, DamageDeliverInfo.BuffInfo(-1, SBuff.SystemPlayerBonfireHealInfo()));
    }
    public void Play(EntityCharacterPlayer _Player)
    {
        base.Play();
        SetInteractable(false);
        OnFireLit(_Player);
    }

    private void OnDisable()
    {
        if (!m_Target)
            return;

        OnDistinguish();
    }

    void OnFireLit(EntityCharacterPlayer _Player)       //Fire On,Smoke On,Light On
    {
        m_Target = _Player;

        m_healBuffCheck = 0f;
        m_distinguishCheck = 8f;

        m_FireParticles.Play();
        tf_Light.SetActivate(true);
    }

    void OnDistinguish()        //Light Low,Fire Off,Start Distinguish
    {
        Debug.Log("distinguish");
        m_Target = null;
        m_FireParticles.Stop();
        tf_Light.SetActivate(false);
    }

    private void Update()
    {
        if (!m_Target)
            return;

        if (m_distinguishCheck > 0)
        {
            m_distinguishCheck -= Time.deltaTime;
            if (m_distinguishCheck < 0)
            {
                OnDistinguish();
                return;
            }
        }

        if(m_healBuffCheck>0)
        {
            m_healBuffCheck -= Time.deltaTime;
            return;
        }
        m_healBuffCheck = .4f;
        m_Target.m_HitCheck.TryHit(m_HealInfo);
    }
}
