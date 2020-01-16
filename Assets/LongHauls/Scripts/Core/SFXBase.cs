using System;
using UnityEngine;
public class SFXBase :PoolObjectMono<int> {
    public const int I_SFXStopExternalDuration= 4;
    public int I_SourceID { get; private set; } = -1;
    protected float f_delayDuration { get; private set; }
    protected float f_playDuration { get; private set; }
    protected float f_lifeDuration { get; private set; }

    protected float f_lifeTimeCheck { get; private set; }
    public bool B_Playing { get; private set; }
    protected bool B_Delay { get; private set; }
    protected virtual bool m_Loop => true;
    protected virtual bool m_AutoStop => true;
    protected virtual bool m_AutoRecycle => true;
    protected virtual bool m_ScaledDeltaTime => true;
    public float f_playTimeLeft => f_lifeTimeCheck - I_SFXStopExternalDuration;
    public float f_delayTimeLeft { get; private set; }
    public float f_delayLeftScale => f_delayTimeLeft>0? (f_delayTimeLeft / f_delayDuration):0;
    protected bool b_looping => m_Loop && B_Playing && f_playDuration <= 0f;
    Transform m_AttachTo;
    Vector3 m_localPos, m_localDir;
    public override void OnPoolItemInit(int _identity, Action<int, MonoBehaviour> _OnRecycle)
    {
        base.OnPoolItemInit(_identity, _OnRecycle);
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
        f_delayTimeLeft = f_delayDuration;
        SetLifeTime(f_playDuration + f_delayDuration);
    }

    protected void SetLifeTime(float lifeDuration)
    {
        f_lifeDuration = lifeDuration + I_SFXStopExternalDuration;
        f_lifeTimeCheck = f_lifeDuration;
    }

    protected virtual void OnPlay()
    {
        B_Delay = false;
        B_Playing = true;
    }

    protected virtual void OnStop()
    {
        f_lifeTimeCheck = I_SFXStopExternalDuration;
        B_Delay = false;
        B_Playing = false;
    }

    protected virtual void OnRecycle()
    {
        m_AttachTo = null;
        DoItemRecycle();
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
        float deltaTime = m_ScaledDeltaTime ? Time.deltaTime : Time.unscaledDeltaTime;
        if (m_AttachTo)
        {
            transform.position = m_AttachTo.TransformPoint(m_localPos);
            transform.rotation = Quaternion.LookRotation(m_AttachTo.TransformDirection(m_localDir));
        }

        if (B_Delay && f_delayTimeLeft >= 0)
        {
            f_delayTimeLeft -= deltaTime;
            if (f_delayTimeLeft < 0)
                OnPlay();
        }

        if (!m_AutoStop && !m_AutoRecycle)
            return;

        if(!b_looping)
            f_lifeTimeCheck -= deltaTime;

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

    protected virtual void EDITOR_DEBUG(){}
#if UNITY_EDITOR
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
