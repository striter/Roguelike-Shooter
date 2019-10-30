using GameSetting;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIM_ActionUpgrade : UIMessageBoxBase {
    UIGI_ActionItemRichIntro m_ItemBefore,m_ItemAfter;
    UIT_TextExtend m_Amount;
    protected override void Init()
    {
        base.Init();
        m_ItemBefore = tf_Container.Find("ActionItemBefore").GetComponent<UIGI_ActionItemRichIntro>();
        m_ItemAfter = tf_Container.Find("ActionItemAfter").GetComponent<UIGI_ActionItemRichIntro>();
        m_ItemBefore.Init();
        m_ItemAfter.Init();
        m_Amount = tf_Container.Find("Intro/Coin/Amount").GetComponent<UIT_TextExtend>();
    }
    public void Play(int amount, ActionBase action, Action OnConfirmClick)
    {
        base.Play(OnConfirmClick);
        ActionBase newacton = GameDataManager.CreateAction(action.m_Index, action.m_rarity);
        newacton.Upgrade();
        string changed = "B9FE00FF";
        string unChanged = "FFDA6BFF";
        bool costChanged = action.I_Cost != newacton.I_Cost;
        bool durationChanged = action.F_Duration != newacton.F_Duration;
        bool value1Changed = action.Value1 != newacton.Value1;
        bool value2Changed = action.Value2 != newacton.Value2;
        bool value3Changed = action.Value3 != newacton.Value3;
        m_Amount.text = amount.ToString();
        m_ItemBefore.SetRichIntro(action,unChanged,unChanged, unChanged, unChanged, unChanged);
        m_ItemAfter.SetRichIntro(newacton,costChanged?changed:unChanged,durationChanged? changed : unChanged,value1Changed? changed : unChanged,value2Changed? changed : unChanged,value3Changed? changed : unChanged);
    }
}
