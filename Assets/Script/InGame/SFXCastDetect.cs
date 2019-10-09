using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;
[RequireComponent(typeof(EntityDetector))]
public class SFXCastDetect : SFXCast {
    EntityDetector m_detector;
    Transform m_Model;
    public override void Init(int _sfxIndex)
    {
        base.Init(_sfxIndex);
        m_detector=GetComponent<EntityDetector>();
        m_detector.Init(OnDetect);
        m_Model = transform.Find("Model");
    }

    public override void Play(DamageDeliverInfo buffInfo)
    {
        base.Play(buffInfo);
        m_detector.SetPlay(true);
        m_Model.SetActivate(true);
    }

    void OnDetect(HitCheckEntity entity, bool enter)
    {
        if (!enter)
            return;

        if (GameManager.B_CanDamageEntity(entity, I_SourceID))
            PlayDelayed();
    }

    public override void PlayDelayed()
    {
        m_detector.SetPlay(false);
        m_Model.SetActivate(false);
        base.PlayDelayed();
    }

    protected override void EDITOR_DEBUG()
    {
        base.EDITOR_DEBUG();
        if (F_DelayDuration <= 0)
            Debug.LogError("Cast Detect Delay Duration Cannot Less Or Equals 0!");
    }
}
