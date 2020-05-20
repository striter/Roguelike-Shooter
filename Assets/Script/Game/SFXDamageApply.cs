using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
public class SFXDamageApply : SFXDamageBase {
    public void Play(int I_SourceID,DamageInfo buffInfo,EntityCharacterBase applyTarget)
    {
        base.PlaySFX(I_SourceID,0f,0f,true);
        applyTarget.m_HitCheck.TryHit(buffInfo);
    }
}
