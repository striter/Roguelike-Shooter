using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
using TSpecialClasses;
using System;

public class InteractActionChest : InteractGameBase
{
    public override enum_Interaction m_InteractType => enum_Interaction.ActionChest;
    public override bool B_InteractOnce => false;
    public bool m_StartChest { get; private set; } = false;
    public override string m_ExternalLocalizeKeyJoint =>"_"+(m_StartChest?"Start":"Battle");
    AnimationControlBase m_Animation;
    List<ActionBase> m_Actions;
    int m_SelectAmount;
    public override void OnPoolItemInit(enum_Interaction identity, Action<enum_Interaction, MonoBehaviour> OnRecycle)
    {
        base.OnPoolItemInit(identity, OnRecycle);
        m_Animation = new AnimationControlBase(GetComponentInChildren<Animation>());
    }

    public void Play(List<ActionBase> _actions, int selectAmount, bool _startChest)
    {
        base.Play();
        m_Actions = _actions;
        m_SelectAmount = selectAmount;
        m_StartChest = _startChest;
        m_Animation.SetPlayPosition(true);
    }

    protected override void OnInteractSuccessful(EntityCharacterPlayer _interactTarget)
    {
        SetInteractable(false);
        m_Animation.Play(true);
        GameUIManager.Instance.ShowGameControlPage<UI_ActionAcquire>(true).Play(m_Actions,_interactTarget, m_SelectAmount,true);
    }
    void OnKeyAnim()
    {
    }
}
