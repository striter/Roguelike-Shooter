using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameSetting;
using System;

public class UIM_ActionRemove : UIMessageBoxBase {
    UIGI_ActionItemRichIntro m_Item;
    UIT_TextExtend m_Amount;
    protected override void Init()
    {
        base.Init();
        m_Item = tf_Container.Find("ActionItem").GetComponent<UIGI_ActionItemRichIntro>();
        m_Item.Init();
        m_Amount =tf_Container.Find("Intro/Coin/Amount").GetComponent<UIT_TextExtend>();
    }
    public void Play(int amount,ActionBase action,Action OnConfirmClick)
    {
        base.Play(OnConfirmClick);
        m_Amount.text = amount.ToString();
        m_Item.SetCommonIntro(action, "FFDA6BFF");
    }
}
