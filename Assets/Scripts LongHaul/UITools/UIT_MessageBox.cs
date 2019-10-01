using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class UIT_MessageBox : SimpleSingletonMono<UIT_MessageBox> {

    Transform tf_Container;
    UIT_TextExtend m_title,m_Intro;
    Button btn_Confirm;
    UIT_TextExtend txt_Confirm;
    Action OnConfirmClick;
    protected override void Awake()
    {
        base.Awake();
        this.SetActivate(false);
        tf_Container = transform.Find("Container");
        m_title = tf_Container.Find("Title").GetComponent<UIT_TextExtend>();
        m_Intro = tf_Container.Find("Intro").GetComponent<UIT_TextExtend>();
        btn_Confirm = tf_Container.Find("Confirm").GetComponent<Button>();
        btn_Confirm.onClick.AddListener(OnConfirm);
        txt_Confirm = tf_Container.Find("Confirm/Text").GetComponent<UIT_TextExtend>();
        tf_Container.Find("Cancel").GetComponent<Button>().onClick.AddListener(OnCancel);
    }
    public void Begin(string titleKey,string introKey, string confirmKey,Action _OnConfirmClick)
    {
        this.SetActivate(true);
        m_title.localizeText = titleKey;
        m_Intro.localizeText = introKey;
        txt_Confirm.localizeText = confirmKey;
        OnConfirmClick = _OnConfirmClick;
    }
    void OnConfirm()
    {
        this.SetActivate(false);
        OnConfirmClick();
    }
    void OnCancel()
    {
        this.SetActivate(false);
    }
}
