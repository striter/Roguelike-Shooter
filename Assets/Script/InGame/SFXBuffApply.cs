using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
public class SFXBuffApply : SFXParticles {
    public int I_BuffIndex;
    public void Play(int I_SourceID,SBuff buffInfo,Transform attachTo,EntityBase applyTarget)
    {
        base.PlaySFX(I_SourceID,buffInfo.m_ExpireDuration);
        transform.SetParent(attachTo);
        transform.position = attachTo.position;
        transform.rotation = Quaternion.LookRotation(Vector3.up);
        applyTarget.OnReceiveBuff(I_BuffIndex);
    }
    protected override void OnPlayFinished()
    {
        transform.SetParent(null);
        base.OnPlayFinished();
    }
}
