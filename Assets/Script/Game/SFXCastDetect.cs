using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;
[RequireComponent(typeof(EntityDetector))]
public class SFXCastDetect : SFXCastDetonate {
    public float F_DurationSelfDetonate;
    EntityDetector m_detector;
    public override void OnPoolItemInit(int identity)
    {
        base.OnPoolItemInit(identity);
        m_detector = GetComponent<EntityDetector>();
        m_detector.Init(OnDetect);
    }

    protected override void OnPoolItemEnable()
    {
        base.OnPoolItemEnable();
        TBroadCaster<enum_BC_GameStatus>.Add(enum_BC_GameStatus.OnBattleFinish, OnDetectEntity);
    }

    protected override void OnPoolItemDisable()
    {
        base.OnPoolItemDisable();
        TBroadCaster<enum_BC_GameStatus>.Remove(enum_BC_GameStatus.OnBattleFinish, OnDetectEntity);
    }

    public override void Play(DamageDeliverInfo buffInfo)
    {
        base.Play(buffInfo);
        m_detector.SetPlay(true);
        base.PlaySFX(I_SourceID, F_PlayDuration, F_DurationSelfDetonate);
    }

    void OnDetect(HitCheckEntity entity, bool enter)
    {
        if (enter&& GameManager.B_CanSFXDamageEntity(entity, I_SourceID))
            OnDetectEntity();
    }

    void OnDetectEntity()
    {
        PlaySFX(I_SourceID, F_PlayDuration, F_DelayDuration);
        m_detector.SetPlay(false);
    }

    protected override void EDITOR_DEBUG()
    {
        base.EDITOR_DEBUG();
        if (F_DelayDuration <= 0)
            Debug.LogError("Cast Detect Delay Duration Cannot Less Or Equals 0!");
    }
}
