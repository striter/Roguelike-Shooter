using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
public class SFXBuffApply : SFXWeaponBase {
    public int I_BuffIndex;
    public void Play(int I_SourceID,SBuff buffInfo,Transform attachTo,EntityCharacterBase applyTarget)
    {
        base.PlayUncontrolled(I_SourceID,buffInfo.m_ExpireDuration);
        transform.SetParent(attachTo);
        transform.position = attachTo.position;
        transform.localRotation = Quaternion.identity;
        applyTarget.m_HitCheck.TryHit(new DamageInfo(I_SourceID).AddPresetBuff(I_BuffIndex));
    }
}
