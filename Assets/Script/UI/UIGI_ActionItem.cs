using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameSetting;
using System;

public class UIGI_ActionItem : UIT_GridItem {
    Image m_ActionImage;
    Image m_TypeIcon,m_TypeBottom;
    UIC_RarityLevel_BG m_Rarity;
    Image m_Costable;
    UIT_TextExtend m_Cost, m_Name;
    UIT_EventTriggerListener m_TriggerListener;
    Action<int> OnClick;
    Action OnPressDuration;
    public override void Init(UIT_GridController parent)
    {
        base.Init(parent);
        m_ActionImage = tf_Container.Find("Icon/Image").GetComponent<Image>();
        m_TypeIcon = tf_Container.Find("Type/Icon").GetComponent<Image>();
        m_TypeBottom = tf_Container.Find("Type/Bottom").GetComponent<Image>();
        m_Rarity = new UIC_RarityLevel_BG(tf_Container.Find("ActionRarity"));
        m_Costable = tf_Container.Find("Cost").GetComponent<Image>();
        m_Cost = tf_Container.Find("Cost/Amount").GetComponent<UIT_TextExtend>();
        m_Name = tf_Container.Find("Name").GetComponent<UIT_TextExtend>();
        m_TriggerListener = tf_Container.Find("TriggerListener").GetComponent<UIT_EventTriggerListener>();
        m_TriggerListener.D_OnPress = OnPress;
    }
    
    public void SetInfo(ActionBase actionInfo,Action<int> _OnClick,Action _OnPressDuration)
    {
        OnClick = _OnClick;
        OnPressDuration = _OnPressDuration;
        m_Cost.text = actionInfo.m_ActionType == enum_ActionType.WeaponPerk ? "" : actionInfo.I_Cost.ToString();
        m_Name.localizeText = actionInfo.GetNameLocalizeKey();
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
