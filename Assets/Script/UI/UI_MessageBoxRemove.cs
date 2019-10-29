using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameSetting;
using System;

public class UI_MessageBoxRemove : UIT_MessageBox {
    UIGI_ActionItemRichIntro m_Item;
    UIT_TextExtend m_Amount;
    protected override void Awake()
    {
        base.Awake();
        m_Item = tf_Container.Find("ActionItem").GetComponent<UIGI_ActionItemRichIntro>();
        m_Item.Init();
        m_Amount =tf_Container.Find("Intro/Coin/Amount").GetComponent<UIT_TextExtend>();
    }
    public void Play(int amount,ActionBase action,Action OnConfirmClick)
    {
        base.Begin(OnConfirmClick);
        m_Amount.text = amount.ToString();
        m_Item.SetInfo(action, "FFDA6BFF");
    }
}
