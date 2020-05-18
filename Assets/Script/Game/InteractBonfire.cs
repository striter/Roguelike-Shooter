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
    TimerBase m_DistinguishCheck=new TimerBase(5f);
    EntityCharacterPlayer m_Interactor;
    public override void OnPoolInit(enum_Interaction identity, Action<enum_Interaction, MonoBehaviour> OnRecycle)
    {
        base.OnPoolInit(identity, OnRecycle);
        m_FireParticles = new TSpecialClasses.ParticleControlBase(transform.Find("Fire"));
        tf_Light = transform.Find("Light");
    }
    public new InteractBonfire Play()
    {
        base.Play();
        OnDistinguish();
        return this;
    }

    protected override bool OnInteractedContinousCheck(EntityCharacterPlayer _interactor)
    {
        base.OnInteractedContinousCheck(_interactor);
        m_Interactor = _interactor;
        GameUIManager.Instance.ShowPage<UI_BonfireSelection>(true,true, 0f).Play(OnBonfireSelect);
        return false;
    }

    void OnBonfireSelect(bool lit)
    {
        if(lit)
        {
            m_DistinguishCheck.Replay();
            m_Interactor.m_HitCheck.TryHit(new DamageInfo(-1, enum_DamageIdentity.Environment).AddPresetBuff(102));
            m_FireParticles.Play();
            tf_Light.SetActivate(true);
        }
        else
        {
            GameObjectManager.PlayMuzzle(-1,transform.position,Vector3.up, 10021);
            m_Interactor.m_CharacterInfo.OnActionPerkAcquire(0);
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
