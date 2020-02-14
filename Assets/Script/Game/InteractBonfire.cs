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
    EntityCharacterPlayer m_Interactor;
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
        m_Interactor = _interactor;
        GameUIManager.Instance.ShowPage<UI_BonfireSelection>(true, 0f).Play(OnBonfireSelect);
        return false;
    }

    void OnBonfireSelect(bool lit)
    {
        if(lit)
        {
            m_DistinguishCheck.Reset();
            m_Interactor.m_HitCheck.TryHit(m_HealInfo);
            m_FireParticles.Play();
            tf_Light.SetActivate(true);
        }
        else
        {
            GameObjectManager.PlayMuzzle(-1,transform.position,Vector3.up, 10021);
            m_Interactor.m_CharacterInfo.OnActionBuffAcquire(ActionDataManager.CreateActionBuff(10003));
        }
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
