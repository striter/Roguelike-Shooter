using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;
[RequireComponent(typeof(EntityDetector))]
public class SFXCastDetect : SFXCastDetonate {
    public float F_DurationSelfDetonate;
    EntityDetector m_detector;
    public override void Init(int _sfxIndex)
    {
        base.Init(_sfxIndex);
        m_detector=GetComponent<EntityDetector>();
        m_detector.Init(OnDetect);
    }

    public override void Play(DamageDeliverInfo buffInfo)
    {
        base.Play(buffInfo);
        m_detector.SetPlay(true);
        base.PlaySFX(I_SourceID, F_PlayDuration, F_DurationSelfDetonate);
    }

    void OnDetect(HitCheckEntity entity, bool enter)
    {
        if (!enter)
            return;

        if (GameManager.B_CanDamageEntity(entity, I_SourceID))
        {
            base.PlaySFX(I_SourceID, F_PlayDuration, F_DelayDuration);
            m_detector.SetPlay(false);
        }
    }

    protected override void EDITOR_DEBUG()
    {
        base.EDITOR_DEBUG();
        if (F_DelayDuration <= 0)
            Debug.LogError("Cast Detect Delay Duration Cannot Less Or Equals 0!");
    }
}
