using System;
using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class InteractTeleport : InteractGameQuadrant  {
    protected override bool E_InteractOnEnable => false;
    public bool m_Enable { get; private set; }
    Animator m_Animator;
    int hs_Unlock = Animator.StringToHash("b_unlock");
    public override void OnPoolItemInit(enum_Interaction identity, Action<enum_Interaction, MonoBehaviour> OnRecycle)
    {
        base.OnPoolItemInit(identity, OnRecycle);
        m_Animator = GetComponent<Animator>();
    }
    public void SetPlay(bool play)
    {
        m_Animator.SetBool(hs_Unlock, play);
    } 
    public InteractTeleport Play(int chunkIndex,enum_LevelStyle style)
    {
        base.PlayQuadrant(chunkIndex);
        transform.Find("Model/Glow").GetComponent<Renderer>().sharedMaterial.color = TCommon.GetHexColor(style.GetTeleportHex());
        return this;
    }
}
