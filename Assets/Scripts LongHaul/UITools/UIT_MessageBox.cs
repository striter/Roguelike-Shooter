using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class UIT_MessageBox : SimpleSingletonMono<UIT_MessageBox> {

    Transform tf_Container;
    UIT_TextLocalization m_title,m_Intro;
    Button btn_Confirm;
    UIT_TextLocalization txt_Confirm;
    Action OnConfirmClick;
    protected override void Awake()
    {
        base.Awake();
        this.SetActivate(false);
        tf_Container = transform.Find("Container");
        m_title = tf_Container.Find("Title").GetComponent<UIT_TextLocalization>();
        m_Intro = tf_Container.Find("Intro").GetComponent<UIT_TextLocalization>();
        btn_Confirm = tf_Container.Find("Confirm").GetComponent<Button>();
        txt_Confirm = tf_Container.Find("Confirm/Text").GetComponent<UIT_TextLocalization>();

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
        OnConfirm();
    }
    void OnCancel()
    {
        this.SetActivate(false);
    }
}
