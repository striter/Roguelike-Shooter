using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
public class SFXBuffApply : SFXWeaponBase {
    public int I_BuffIndex;
    public void Play(int I_SourceID,SBuff buffInfo,EntityCharacterBase applyTarget)
    {
        base.PlaySFX(I_SourceID,buffInfo.m_ExpireDuration,0f,true);
        applyTarget.m_HitCheck.TryHit(new DamageInfo(I_SourceID).AddPresetBuff(I_BuffIndex));
    }
}
