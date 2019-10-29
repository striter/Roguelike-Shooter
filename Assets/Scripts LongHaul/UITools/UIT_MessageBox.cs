using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class UIT_MessageBox : MonoBehaviour {
    protected Transform tf_Container { get; private set; }
    Button btn_Confirm;
    Action OnConfirmClick;
    protected virtual void Awake()
    {
        this.SetActivate(false);
        tf_Container = transform.Find("Container");
        btn_Confirm = tf_Container.Find("Confirm").GetComponent<Button>();
        btn_Confirm.onClick.AddListener(OnConfirm);
        tf_Container.Find("Cancel").GetComponent<Button>().onClick.AddListener(OnCancel);
    }
    protected void Begin(Action _OnConfirmClick)
    {
        this.SetActivate(true);
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
