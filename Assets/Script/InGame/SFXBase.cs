using System;
using UnityEngine;
using GameSetting;
public class SFXBase : MonoBehaviour {
    public int I_SFXIndex { get; private set; } = -1;
    public bool b_Playing { get; private set; }
    public int I_SourceID { get; private set; }
    protected float f_duration;
    protected float f_TimeCheck;
    protected float f_timeLeft;
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
        f_duration = duration;
        f_TimeCheck = Time.time + duration;
        I_SourceID = sourceID;
        OnSFXPlayFinished = _OnSFXPlayFinished;
        b_Playing = true;
    }

    protected virtual void Update()
    {
        if (!b_Playing)
            return;
        f_timeLeft =   f_TimeCheck- Time.time;

        if (b_Playing&& f_timeLeft<0)
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
        ObjectManager.RecycleSFX(I_SFXIndex, this);
    }

}
