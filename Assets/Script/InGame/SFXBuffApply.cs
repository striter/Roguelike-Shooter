using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
public class SFXBuffApply : SFXParticles {
    public int I_BuffIndex;
    public void Play(int I_SourceID,SBuff buffInfo,Transform attachTo,EntityCharacterBase applyTarget)
    {
        base.Play(I_SourceID,buffInfo.m_ExpireDuration);
        transform.SetParent(attachTo);
        transform.position = attachTo.position;
        transform.localRotation = Quaternion.identity;
        applyTarget.m_HitCheck.TryHit(new DamageInfo(0, enum_DamageType.Basic,DamageDeliverInfo.BuffInfo(I_SourceID,I_BuffIndex)));
    }
}
