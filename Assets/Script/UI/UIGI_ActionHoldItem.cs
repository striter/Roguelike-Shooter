using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameSetting;
using System;

public class UIGI_ActionHoldItem : UIT_GridItem {
    UIT_TextExtend m_Name,m_Level,m_Cost;
    UIT_EventTriggerListener m_TriggerListener;
    Action<int> OnClick;
    Action OnPressDuration;
    public override void Init(UIT_GridController parent)
    {
        base.Init(parent);
        m_Cost = tf_Container.Find("Cost").GetComponent<UIT_TextExtend>();
        m_Name = tf_Container.Find("Name").GetComponent<UIT_TextExtend>();
        m_Level = tf_Container.Find("Level").GetComponent<UIT_TextExtend>();
        m_TriggerListener = tf_Container.Find("TriggerListener").GetComponent<UIT_EventTriggerListener>();
        m_TriggerListener.D_OnPress = OnPress;
    }

    public void SetInfo(ActionBase actionInfo,Action<int> _OnClick,Action _OnPressDuration)
    {
        OnClick = _OnClick;
        OnPressDuration = _OnPressDuration;
        m_Cost.text = actionInfo.m_ActionExpireType== enum_ActionExpireType.AfterWeaponSwitch?"":actionInfo.I_ActionCost.ToString();
        m_Name.localizeText = actionInfo.GetNameLocalizeKey();
        m_Level.localizeText = actionInfo.m_rarity.GetLocalizeKey();
    }

    bool b_pressing;
    float f_pressDuration;
    private void Update()
    {
        if (!b_pressing)
            return;
        f_pressDuration += Time.deltaTime;

        if (f_pressDuration > .5f)
        {
            OnPressDuration.Invoke();
            b_pressing = false;
        }
    }
    void OnPress(bool down,Vector2 deltaPos)
    {
        if (!down && f_pressDuration < .5f)
            OnClick.Invoke(I_Index);
        b_pressing = down;
        f_pressDuration = 0;
    }
}
