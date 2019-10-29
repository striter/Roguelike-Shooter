using GameSetting;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_MessageBoxUpgrade : UIT_MessageBox {
    UIGI_ActionItemRichIntro m_ItemBefore,m_ItemAfter;
    UIT_TextExtend m_Amount;
    protected override void Awake()
    {
        base.Awake();
        m_ItemBefore = tf_Container.Find("ActionItemBefore").GetComponent<UIGI_ActionItemRichIntro>();
        m_ItemAfter = tf_Container.Find("ActionItemAfter").GetComponent<UIGI_ActionItemRichIntro>();
        m_ItemBefore.Init();
        m_ItemAfter.Init();
        m_Amount = tf_Container.Find("Intro/Coin/Amount").GetComponent<UIT_TextExtend>();
    }
    public void Play(int amount, ActionBase action, Action OnConfirmClick)
    {
        base.Begin(OnConfirmClick);
        m_Amount.text = amount.ToString();
        m_ItemBefore.SetInfo(action, "FFDA6BFF");
        ActionBase newacton = GameDataManager.CreateAction(action.m_Index,action.m_rarity);
        newacton.Upgrade();
        m_ItemAfter.SetInfo(newacton, "B9FE00FF");
    }
}
