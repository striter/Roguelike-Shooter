using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
public class InteractActionChest : InteractBase {
    Animation m_Animation;
    string m_clipName;
    List<ActionBase> m_Actions;
    EntityCharacterPlayer m_Interactor;
    public override enum_Interaction m_InteractType => enum_Interaction.ActionChest;
    public override void Init()
    {
        base.Init();
        m_Animation = GetComponentInChildren<Animation>();
        m_clipName = m_Animation.clip.name;
    }
    public void Play(List<ActionBase> _actions)
    {
        base.Play();
        m_Actions = _actions;
        m_Animation[m_clipName].normalizedTime = 0;
        m_Animation[m_clipName].speed = 0;
        m_Animation.Play();
    }
    protected override void OnInteractSuccessful(EntityCharacterPlayer _interactTarget)
    {
        base.OnInteractSuccessful(_interactTarget);
        m_Interactor = _interactTarget;
        UI_ActionAcquire page = UIManager.Instance.ShowPage<UI_ActionAcquire>(true);
        if (page == null)
            return;
        page.Play(m_Actions, OnActionSelectConfirm);
    }
    void OnKeyAnim()
    {
    }
    void OnActionSelectConfirm(int index)
    {
        m_Interactor.OnInteractCheck(this, false);
        SetInteractable(false);
        m_Interactor.m_PlayerInfo.AddStoredAction(m_Actions[index]);
        m_Animation[m_clipName].speed = 1;
        m_Animation.Play();
    }
}
