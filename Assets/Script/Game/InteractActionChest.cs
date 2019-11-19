using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
using TSpecialClasses;
public class InteractActionChest : InteractGameBase
{
    public override enum_Interaction m_InteractType => enum_Interaction.ActionChest;
    public override bool B_InteractOnce => false;
    public bool m_StartChest { get; private set; } = false;
    public override string m_ExternalLocalizeKeyJoint =>"_"+(m_StartChest?"Start":"Battle");
    AnimationControlBase m_Animation;
    List<ActionBase> m_Actions;
    int m_SelectAmount;
    EntityCharacterPlayer m_Interactor;
    TSpecialClasses.ParticleControlBase m_Particles;
    public override void OnPoolItemInit(enum_Interaction identity)
    {
        base.OnPoolItemInit(identity);
        m_Animation = new AnimationControlBase(GetComponentInChildren<Animation>());
        m_Particles = new ParticleControlBase(transform);
    }
    public void Play(List<ActionBase> _actions,int selectAmount,bool _startChest)
    {
        base.Play();
        m_Actions = _actions;
        m_SelectAmount = selectAmount;
        m_StartChest = _startChest;
        m_Animation.SetPlayPosition(true);
        m_Particles.Play();
    }
    protected override void OnInteractSuccessful(EntityCharacterPlayer _interactTarget)
    {
        m_Interactor = _interactTarget;
        SetInteractable(false);
        m_Animation.Play(true);
        m_Particles.Stop();
        GameUIManager.Instance.ShowGameControlPage<UI_ActionAcquire>(true).Play(m_Actions, OnActionSelectConfirm, m_SelectAmount);
    }
    void OnActionSelectConfirm(int index)
    {
        base.OnInteractSuccessful(m_Interactor);
        m_Interactor.m_PlayerInfo.AddStoredAction(m_Actions[index]);
    }
    void OnKeyAnim()
    {
    }
}
