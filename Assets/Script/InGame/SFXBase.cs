using System;
using UnityEngine;
using GameSetting;
public class SFXBase : MonoBehaviour {
    public enum_SFX E_Type { get; private set; } = enum_SFX.Invalid;

    public virtual void Init(enum_SFX type)
    {
        E_Type = type;
    }

    public void Start()
    {
        if (E_Type == enum_SFX.Invalid)
            Debug.LogError("Please Init Before Start!" + gameObject.name.ToString());
    }
    public int I_SourceID { get; private set; }
    protected float f_duration;
    protected float f_TimeCheck;
    Action OnSFXPlayFinished;

    protected void Play(int sourceID,float duration,Action _OnSFXPlayFinished=null)
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
        ObjectManager.RecycleSFX(E_Type, this);
    }
}
