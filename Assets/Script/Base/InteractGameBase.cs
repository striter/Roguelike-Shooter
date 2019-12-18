﻿using GameSetting;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractGameBase : InteractBase,ObjectPoolItem<enum_Interaction> {

    protected virtual bool B_SelfRecycleOnInteract => false;
    public AudioClip AC_OnPlay, AC_OnInteract;
    public int I_MuzzleOnInteract;

    public virtual void OnPoolItemInit(enum_Interaction identity, Action<enum_Interaction, MonoBehaviour> OnRecycle)
    {
    }

    protected override void Play()
    {
        base.Play();
        if (AC_OnPlay)
            AudioManager.Instance.Play3DClip(-1, AC_OnPlay, false, transform.position);
    }
    
    protected override bool OnInteractOnceCanKeepInteract(EntityCharacterPlayer _interactTarget)
    {
        GameObjectManager.PlayMuzzle(_interactTarget.m_EntityID, transform.position, transform.up, I_MuzzleOnInteract, AC_OnInteract);
        if (B_SelfRecycleOnInteract)
            OnRecycle();
        return base.OnInteractOnceCanKeepInteract(_interactTarget);
    }

    protected void OnRecycle()
    {
        SetInteractable(false);
        GameObjectManager.RecycleInteract(this);
    }

}
