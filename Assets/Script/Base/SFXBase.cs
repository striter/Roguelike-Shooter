using System;
using UnityEngine;
using GameSetting;
public class SFXBase : MonoBehaviour {
    public int I_SFXIndex { get; private set; } = -1;
    public bool b_Playing { get; private set; }
    public int I_SourceID { get; private set; }
    protected float f_duration { get; private set; }
    protected float f_lifeTime { get; private set; }
    protected bool B_Playing { get; private set; }
    protected virtual bool m_AutoRecycle => true;
    protected virtual bool m_AutoStop => true;
    protected float f_playTimeLeft => f_lifeTime - GameConst.F_SFXMaxStopDuration;
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
        B_Playing = true;
    }

    protected void SetLifeTime(float duration)
    {
        f_duration = duration+ GameConst.F_SFXMaxStopDuration;
        f_lifeTime = f_duration;
    }

    protected virtual void Update()
    {
        f_lifeTime -= Time.deltaTime;
        if (B_Playing&&m_AutoStop && f_playTimeLeft < 0)
            OnStop();

        if (m_AutoStop && f_lifeTime < 0)
            OnRecycle();
    }

    public virtual void OnStop()
    {
        B_Playing = false;
    }

    protected  virtual void OnRecycle()
    {
        OnSFXPlayFinished?.Invoke();
        GameObjectManager.RecycleSFX(I_SFXIndex, this);
    }

    public void Recycle()
    {
        OnRecycle();
    }
    public void Stop()
    {

    }
}
