﻿using System;
using System.Collections;
using System.Collections.Generic;
using GameSetting;
using TSpecialClasses;
using UnityEngine;

public class WeaponCastAnim : WeaponCastBase {
    ValueChecker<float> m_ScaleChecker = new ValueChecker<float>(-1);

    protected float GetMeleeSize() => m_Attacher.m_CharacterInfo.F_Cast_Melee_SizeMultiply;
    Vector4 m_BaseSize;
    public override void OnPoolItemInit(enum_PlayerWeaponIdentity _identity, Action<enum_PlayerWeaponIdentity, MonoBehaviour> _OnRecycle)
    {
        base.OnPoolItemInit(_identity, _OnRecycle);
        SFXCast cast = GameObjectManager.GetSFXWeaponData<SFXCast>(m_BaseSFXWeaponIndex);
        m_BaseSize = cast.V4_CastInfo;
    }

    protected override void OnAutoTrigger()
    {
        OnAttacherAnim();
    }
    protected override void OnKeyAnim()
    {
        DoMeleeCast(m_BaseSFXWeaponIndex, GetMeleeSize());
    }

    protected void DoMeleeCast(int castIndex,float castScale=1)
    {
        SFXCast cast = ShowCast(castIndex, m_Attacher.tf_WeaponAim.position);
        cast.V4_CastInfo = m_BaseSize* castScale;
        cast.Play(GetWeaponDamageInfo(m_BaseDamage));
        OnAmmoCost();
    }

    public override void OnAttach(EntityCharacterPlayer _attacher, Transform _attachTo)
    {
        base.OnAttach(_attacher, _attachTo);
        m_ScaleChecker.Check(1);
        transform.localScale = Vector3.one;
    }

    public override void OnDetach()
    {
        base.OnDetach();
        m_ScaleChecker.Check(1);
        transform.localScale = Vector3.one;
    }

    public override void Tick(bool firePausing, float deltaTime)
    {
        base.Tick(firePausing, deltaTime);
        if (m_ScaleChecker.Check(GetMeleeSize()))
            transform.localScale = Vector3.one * m_ScaleChecker.check1;
    }
}