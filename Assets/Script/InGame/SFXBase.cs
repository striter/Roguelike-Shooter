using System;
using UnityEngine;
using GameSetting;
public class SFXBase : MonoBehaviour {
    public int I_SFXIndex { get; private set; } = -1;

    public virtual void Init(int _sfxIndex)
    {
        I_SFXIndex = _sfxIndex;
    }

    public void Start()
    {
        if (I_SFXIndex == -1)
            Debug.LogError("Please Init Before Start!" + gameObject.name.ToString());
    }
    public int I_SourceID { get; private set; }
    protected float f_duration;
    protected float f_TimeCheck;
    Action OnSFXPlayFinished;

    protected void PlaySFX(int sourceID,float duration,Action _OnSFXPlayFinished=null)
    {
        f_duration = duration;
        f_TimeCheck = Time.time + duration;
        I_SourceID = sourceID;
        OnSFXPlayFinished = _OnSFXPlayFinished;
    }

    protected virtual void Update()
    {
        if ( Time.time > f_TimeCheck)
            OnPlayFinished();
    }
    protected  virtual void OnPlayFinished()
    {
        OnSFXPlayFinished?.Invoke();
        ObjectManager.RecycleSFX(I_SFXIndex, this);
    }
    public void Stop()
    {
        OnPlayFinished();
    }
}
