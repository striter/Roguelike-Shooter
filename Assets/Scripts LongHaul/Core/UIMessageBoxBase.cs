using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class UIMessageBoxBase : MonoBehaviour {
    public static Action OnMessageBoxExit;
    public static T Show<T>(Transform _parentTrans) where T : UIMessageBoxBase
    {
        T tempBase = TResources.Instantiate<T>("UI/MessageBoxes/" + typeof(T).ToString(), _parentTrans);
        tempBase.Init();
        return tempBase;
    }

    protected Transform tf_Container { get; private set; }
    Button btn_Confirm;
    Action OnConfirmClick;

    protected virtual void Init()
    {
        tf_Container = transform.Find("Container");
        btn_Confirm = tf_Container.Find("Confirm").GetComponent<Button>();
        btn_Confirm.onClick.AddListener(OnConfirm);
        tf_Container.Find("Cancel").GetComponent<Button>().onClick.AddListener(OnCancel);
        transform.localScale = UIManagerBase.m_PageFitScale;
    }
    protected virtual void OnDestroy()
    {
    }

    protected void Play(Action _OnConfirmClick)
    {
        OnConfirmClick = _OnConfirmClick;
    }
    void OnConfirm()
    {
        OnConfirmClick();
        OnCancel();
    }
    void OnCancel()
    {
        OnMessageBoxExit();
        Destroy(this.gameObject);
    }
}
