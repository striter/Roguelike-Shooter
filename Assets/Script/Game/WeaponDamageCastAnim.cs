using System;
using System.Collections;
using System.Collections.Generic;
using GameSetting;
using TSpecialClasses;
using UnityEngine;

public class WeaponDamageCastAnim : WeaponDamageCast {
    ValueChecker<float> m_ScaleChecker = new ValueChecker<float>(-1);

    protected float GetMeleeSize() => m_Attacher.m_CharacterInfo.F_Cast_Melee_SizeMultiply;
    Vector4 m_BaseSize;
    public override void OnPoolInit(enum_PlayerWeaponIdentity _identity, Action<enum_PlayerWeaponIdentity, MonoBehaviour> _OnRecycle)
    {
        base.OnPoolInit(_identity, _OnRecycle);
        SFXCast cast = GameObjectManager.GetSFXWeaponData<SFXCast>(m_BaseSFXWeaponIndex);
        m_BaseSize = cast.V4_CastInfo;
    }

    protected override void OnKeyAnim()
    {
        base.OnKeyAnim();
        DoMeleeCast(m_BaseSFXWeaponIndex, GetMeleeSize());
    }

    protected void DoMeleeCast(int castIndex,float castScale=1)
    {
        SFXCast cast = ShowCast(castIndex, m_Attacher.tf_WeaponAim.position);
        cast.V4_CastInfo = m_BaseSize* castScale;
        cast.Play(GetWeaponDamageInfo(m_BaseDamage));
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

    public override void WeaponTick(bool firePausing, float deltaTime)
    {
        base.WeaponTick(firePausing, deltaTime);
        if (m_ScaleChecker.Check(GetMeleeSize()))
            transform.localScale = Vector3.one * m_ScaleChecker.value1;
    }

    public override void OnDealtDamage(float amountApply)
    {
        base.OnDealtDamage(amountApply);
        GameManager.SetGameBulletTime(.2f);
    }
}
