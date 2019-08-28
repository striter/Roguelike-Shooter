using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
public class InteractActionChest : InteractBase {
    Animation m_Animation;
    string m_clipName;
    List<ActionBase> m_Actions;
    EntityPlayerBase m_Interactor;
    public override bool B_InteractaOnce => true;
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
    public override bool TryInteract(EntityPlayerBase _interactor)
    {
        base.TryInteract(_interactor);
        m_Interactor = _interactor;
        m_Animation[m_clipName].speed = 1;
        m_Animation.Play();
        return true;
    }

    void OnKeyAnim()
    {
        UIManager.Instance.ShowPage<UI_ActionAcquire>(true).Play(m_Actions,OnActionSelectConfirm);
    }
    void OnActionSelectConfirm(int index)
    {
        m_Interactor.m_PlayerInfo.AddStoredAction(m_Actions[index]);
    }
}
