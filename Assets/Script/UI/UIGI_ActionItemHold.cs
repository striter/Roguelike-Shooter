using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameSetting;
using System;

public class UIGI_ActionItemHold : UIGI_ActionItemBase
{
    Action<int> OnClick;
    UIT_EventTriggerListener m_TriggerListener;
    Action OnPressDuration;

    PlayerInfoManager m_InfoManager;
    ActionBase m_ActionInfo;
    public override void Init()
    {
        base.Init();
        m_TriggerListener = transform.Find("TriggerListener").GetComponent<UIT_EventTriggerListener>();
        m_TriggerListener.D_OnPress = OnPress;
    }
    public void SetInfo(PlayerInfoManager info,ActionBase actionInfo,Action<int> _OnClick,Action _OnPressDuration)
    {
        base.SetInfo(actionInfo);
        OnClick = _OnClick;
        OnPressDuration = _OnPressDuration;
        m_ActionInfo = actionInfo;
        m_InfoManager = info;
        b_pressing = false;
        f_pressDuration = 0f;
    }
    bool b_pressing;
    float f_pressDuration;
    private void Update()
    {
        SetCostable(m_InfoManager.CanCostEnergy(m_ActionInfo.I_Cost));
        if (!b_pressing)
            return;
        f_pressDuration += Time.deltaTime;

        if (f_pressDuration > UIConst.F_UIActionBattlePressDuration)
        {
            OnPressDuration.Invoke();
            b_pressing = false;
        }
    }
    void OnPress(bool down,Vector2 deltaPos)
    {
        if (!down && f_pressDuration < UIConst.F_UIActionBattlePressDuration)
            OnClick.Invoke(I_Index);
        b_pressing = down;
        f_pressDuration = 0;
    }
}
