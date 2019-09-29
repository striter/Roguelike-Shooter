using System;
using UnityEngine;
using GameSetting;
public class SFXBase : MonoBehaviour {
    public int I_SFXIndex { get; private set; } = -1;
    public bool b_Playing { get; private set; }
    public int I_SourceID { get; private set; }
    protected float f_duration { get; private set; }
    protected float f_TimeCheck { get; private set; }
    protected float f_timeLeft => f_duration - f_TimeCheck;
    protected virtual bool m_AutoRecycle => true;
    Action OnSFXPlayFinished;
    public virtual void Init(int _sfxIndex)
    {
        I_SFXIndex = _sfxIndex;
    }

    public void Start()
    {
        if (I_SFXIndex == -1)
            Debug.LogError("Please Init Before Start!" + gameObject.name.ToString());
    }

    protected void PlaySFX(int sourceID,float duration,Action _OnSFXPlayFinished=null)
    {
        SetLifeTime(duration);
        I_SourceID = sourceID;
        OnSFXPlayFinished = _OnSFXPlayFinished;
        b_Playing = true;
    }

    protected virtual void SetLifeTime(float duration)
    {
        f_duration = duration;
        f_TimeCheck = duration;
    }

    protected virtual void Update()
    {
        if (!b_Playing)
            return;
        f_TimeCheck -= Time.deltaTime;
        if (m_AutoRecycle&&f_TimeCheck < 0)
            OnRecycle();
    }

    public void ForceRecycle()
    {
        OnRecycle();
    }
    protected  virtual void OnRecycle()
    {
        b_Playing = false;
        OnSFXPlayFinished?.Invoke();
        GameObjectManager.RecycleSFX(I_SFXIndex, this);
    }

}
