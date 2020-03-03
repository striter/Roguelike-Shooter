using System;
using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class InteractTeleport : InteractGameBase  {
    protected override bool E_InteractOnEnable => false;
    public bool m_Enable { get; private set; }
    Animator m_Animator;
    int hs_T_Activate = Animator.StringToHash("t_activate");
    int hs_T_Unlock = Animator.StringToHash("t_unlock");
    public override void OnPoolItemInit(enum_Interaction identity, Action<enum_Interaction, MonoBehaviour> OnRecycle)
    {
        base.OnPoolItemInit(identity, OnRecycle);
        m_Animator = GetComponent<Animator>();
    }
    public InteractTeleport Play(enum_GameStyle style)
    {
        base.Play();
        transform.Find("Model/Glow").GetComponent<Renderer>().sharedMaterial.color = TCommon.GetHexColor(style.GetTeleportHex());
        return this;
    }
    public void SetPlay(bool play)
    {
        m_Enable = play;
        if (m_Enable)
            m_Animator.SetTrigger(hs_T_Unlock);
    }
    private void OnEnable()
    {
        if (m_Enable)
            m_Animator.SetTrigger(hs_T_Activate);
    }
}
