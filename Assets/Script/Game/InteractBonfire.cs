using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class InteractBonfire : InteractGameBase {
    public override enum_Interaction m_InteractType => enum_Interaction.Bonfire;
    protected override bool B_CanInteract(EntityCharacterPlayer _interactor) => false;
    TSpecialClasses.ParticleControlBase m_FireParticles,m_SmokeParticles;
    TSpecialClasses.AnimationControlBase m_FireDistinguishAnim,m_FireLightAnim;
    EntityCharacterPlayer m_Target;
    DamageInfo m_HealInfo;
    float m_distinguishCheck, m_healBuffCheck;
    public override void OnPoolItemInit(enum_Interaction temp)
    {
        base.OnPoolItemInit(temp);
        m_FireParticles = new TSpecialClasses.ParticleControlBase(transform.Find("Fire"));
        m_SmokeParticles = new TSpecialClasses.ParticleControlBase(transform.Find("Smoke"));
        m_FireDistinguishAnim = new TSpecialClasses.AnimationControlBase(GetComponent<Animation>());
        m_FireLightAnim = new TSpecialClasses.AnimationControlBase(transform.Find("Light").GetComponent<Animation>());
        m_HealInfo = new DamageInfo(0, enum_DamageType.HealthOnly, DamageDeliverInfo.BuffInfo(-1,SBuff.SystemPlayerBonfireHealInfo()));
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
        m_FireDistinguishAnim.SetPlayPosition(false);
    }

    void OnFireLit(EntityCharacterPlayer _Player)       //Fire On,Smoke On,Light On
    {
        m_Target = _Player;

        m_healBuffCheck = 0f;
        m_distinguishCheck = 8f;

        m_FireParticles.Play();
        m_SmokeParticles.Play();
        m_FireLightAnim.Play(true);
    }

    void OnDistinguish()        //Light Low,Fire Off,Start Distinguish
    {
        m_Target = null;
        
        m_FireParticles.Stop();
        m_FireDistinguishAnim.Play(true);
        m_FireLightAnim.Stop();
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
