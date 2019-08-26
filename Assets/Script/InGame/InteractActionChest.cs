﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
public class InteractActionChest : InteractBase {
    Animation m_Animation;
    string m_clipName;
    public override bool B_InteractaOnce => true;
    public override void Init()
    {
        base.Init();
        m_Animation = GetComponentInChildren<Animation>();
        m_clipName = m_Animation.clip.name;
    }
    public new void Play()
    {
        base.Play();
        m_Animation[m_clipName].normalizedTime = 0;
        m_Animation[m_clipName].speed = 0;
        m_Animation.Play();
    }
    public override bool TryInteract(EntityPlayerBase _interactor)
    {
        base.TryInteract(_interactor);
        m_Animation[m_clipName].speed = 1;
        m_Animation.Play();
        return true;
    }

    void OnKeyAnim()
    {
        Debug.Log("Chest Opened???");
    }
}