using System;
using UnityEngine;
using GameSetting;
public class SFXBase : MonoBehaviour {
    public enum_SFX E_Type = enum_SFX.Invalid;

    public void Init(enum_SFX type)
    {
        E_Type = type;
    }

    public void Start()
    {
        if (E_Type == enum_SFX.Invalid)
            Debug.LogError("Please Init Before Start!" + gameObject.name.ToString());
    }

    protected float f_duration;
    float f_TimeCheck;
    Action OnSFXPlayFinished;

    protected virtual void Play(float duration,Action _OnSFXPlayFinished=null)
    {
        f_duration = duration;
        f_TimeCheck = Time.time + duration;
        OnSFXPlayFinished = _OnSFXPlayFinished;
    }

    protected virtual void Update()
    {
        if ( Time.time > f_TimeCheck)
            OnPlayFinished();
    }
    protected void OnPlayFinished()
    {
        OnSFXPlayFinished?.Invoke();
        ObjectManager.RecycleSFX(E_Type, this);
    }
}
