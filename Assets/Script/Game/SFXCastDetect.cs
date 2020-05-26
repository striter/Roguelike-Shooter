using System;
using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;
[RequireComponent(typeof(EntityDetector))]
public class SFXCastDetect : SFXCastDetonate {
    public float F_DurationSelfDetonate;
    EntityDetector m_detector;
    public override void OnPoolInit(int _identity, Action<int, MonoBehaviour> _OnRecycle)
    {
        base.OnPoolInit(_identity, _OnRecycle);
        m_detector = GetComponent<EntityDetector>();
        m_detector.Init(OnDetect);
    }

    public override void Play(DamageInfo damageInfo)
    {
        base.Play(damageInfo);
        m_detector.SetPlay(true);
        base.PlaySFX(m_SourceID, F_PlayDuration, F_DurationSelfDetonate,true);
    }

    void OnDetect(HitCheckEntity entity, bool enter)
    {
        if (enter&& BattleManager.B_CanSFXDamageEntity(entity, m_SourceID))
            OnDetectEntity();
    }

    void OnDetectEntity()
    {
        PlaySFX(m_SourceID, F_PlayDuration, F_DelayDuration,true);
        m_detector.SetPlay(false);
    }
}
