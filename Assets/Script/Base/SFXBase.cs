using System;
using UnityEngine;
using GameSetting;
public class SFXBase : MonoBehaviour {
    public int I_SFXIndex { get; private set; } = -1;
    public bool b_Playing { get; private set; }
    public int I_SourceID { get; private set; }
    protected float f_delayDuration { get; private set; }
    protected float f_playDuration { get; private set; }
    protected float f_lifeDuration { get; private set; }
    protected float f_lifeTimeCheck { get; private set; }
    protected bool B_Playing { get; private set; }
    protected bool B_Delay { get; private set; }
    protected virtual bool m_AutoStop => true;
    protected virtual bool m_AutoRecycle => true;
    protected float f_playTimeLeft => f_lifeTimeCheck - GameConst.F_SFXStopExternalDuration;
    protected float f_delayTimeLeft => f_delayDuration -(f_lifeDuration- f_lifeTimeCheck);
    public virtual void Init(int _sfxIndex)
    {
        I_SFXIndex = _sfxIndex;

#if UNITY_EDITOR
        EDITOR_DEBUG();
#endif
    }
    
    protected void PlaySFX(int sourceID,float playDuration,float delayDuration)
    {
        B_Delay = true;
        B_Playing = false;
        I_SourceID = sourceID;
        f_playDuration = playDuration;
        f_delayDuration = delayDuration;
        SetLifeTime(f_playDuration + f_delayDuration);
        if (f_delayDuration <= 0)
            OnPlay();
    }

    protected void SetLifeTime(float lifeDuration)
    {
        f_lifeDuration = lifeDuration + GameConst.F_SFXStopExternalDuration;
        f_lifeTimeCheck = f_lifeDuration;
    }

    protected virtual void OnPlay()
    {
        B_Delay = false;
        B_Playing = true;
    }

    protected virtual void OnStop()
    {
        f_lifeTimeCheck = GameConst.F_SFXStopExternalDuration;
        B_Playing = false;
    }

    protected virtual void OnRecycle()
    {
        GameObjectManager.RecycleSFX(I_SFXIndex, this);
    }

    protected virtual void Update()
    {
        if (!m_AutoStop && !m_AutoRecycle)
            return;

        if(B_Playing&&f_playDuration>0)
            f_lifeTimeCheck -= Time.deltaTime;

        if (B_Delay &&f_delayDuration>0&& f_delayTimeLeft < 0)
            OnPlay();

        if (m_AutoStop&&B_Playing && f_playTimeLeft < 0)
            OnStop();

        if (m_AutoRecycle&&f_lifeTimeCheck < 0)
            OnRecycle();
    }

    public void Recycle()
    {
        OnRecycle();
    }

    protected virtual void EDITOR_DEBUG()
    {
        if (I_SourceID == -1)
            Debug.LogError("How'd fk SFX SOURCE ID Equals -1");
        if (I_SFXIndex == -1)
            Debug.Log("How'd fk SFX Index ID Equals -1");
    }
}
