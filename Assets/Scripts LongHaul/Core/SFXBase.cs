using System;
using UnityEngine;
using GameSetting;
public class SFXBase : MonoBehaviour {
    public int I_SFXIndex { get; private set; } = -1;
    public int I_SourceID { get; private set; }
    protected float f_delayDuration { get; private set; }
    protected float f_playDuration { get; private set; }
    protected float f_lifeDuration { get; private set; }
    protected float f_lifeTimeCheck { get; private set; }
    public bool B_Playing { get; private set; }
    protected bool B_Delay { get; private set; }
    protected virtual bool m_Loop => true;
    protected virtual bool m_AutoStop => true;
    protected virtual bool m_AutoRecycle => true;
    protected float f_playTimeLeft => f_lifeTimeCheck - GameConst.F_SFXStopExternalDuration;
    protected float f_delayTimeLeft => f_delayDuration -(f_lifeDuration- f_lifeTimeCheck);
    protected bool b_looping => m_Loop && B_Playing && f_playDuration <= 0f;
    Transform m_AttachTo;
    Vector3 m_localPos, m_localDir;

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
        m_AttachTo = null;
        GameObjectManager.RecycleSFX(I_SFXIndex, this);
    }

    public void AttachTo(Transform _attachTo)
    {
        m_AttachTo = _attachTo;
        if (!_attachTo)
            return;
        m_localPos = _attachTo.InverseTransformPoint(transform.position);
        m_localDir = _attachTo.InverseTransformDirection(transform.forward);
    }

    protected virtual void Update()
    {
        if (m_AttachTo)
        {
            transform.position = m_AttachTo.TransformPoint(m_localPos);
            transform.rotation = Quaternion.LookRotation(m_AttachTo.TransformDirection(m_localDir));
        }

        if (!m_AutoStop && !m_AutoRecycle)
            return;

        if(!b_looping)
            f_lifeTimeCheck -= Time.deltaTime;

        if (B_Delay && f_delayTimeLeft < 0)
            OnPlay();

        if (m_AutoStop&&B_Playing && f_playTimeLeft < 0)
            OnStop();

        if (m_AutoRecycle&&f_lifeTimeCheck < 0)
            OnRecycle();
    }


    public void Stop()
    {
        OnStop();
    }
    public void Recycle()
    {
        OnRecycle();
    }
    #if UNITY_EDITOR
    protected virtual void EDITOR_DEBUG()
    {
        if (I_SourceID == -1)
            Debug.LogError("How'd fk SFX SOURCE ID Equals -1");
        if (I_SFXIndex == -1)
            Debug.Log("How'd fk SFX Index ID Equals -1");
    }

    protected Color EDITOR_GizmosColor()
    {
        Color color = Color.red;
        if (B_Playing)
            color = Color.green;
        if (B_Delay)
            color = Color.yellow;
        if (!UnityEditor.EditorApplication.isPlaying)
            color = Color.white;
        return color;
    }
#endif
}
