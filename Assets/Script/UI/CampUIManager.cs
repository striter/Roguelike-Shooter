using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampUIManager : UIManager {
    public static new CampUIManager Instance { get; private set; }
    UIT_CampStatus m_Status;
    protected override void Init()
    {
        base.Init();
        Instance = this;
        btn_Reload.SetActivate(false);

        m_Status = ShowTools<UIT_CampStatus>();
        m_Status.SetInFarm(false);
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        Instance = null;
    }


    public void BeginFarm(Action<bool,Vector2 pos> _OnDragDown, Action<Vector2> _OnDrag,Action _OnExitFarm)
    {
        m_Status.SetInFarm(true);
        tf_Control.SetActivate(false);
        m_TouchDelta.AddDragBinding(_OnDragDown,_OnDrag);
    }
    void EndFarm()
    {
        m_Status.SetInFarm(false);
        tf_Control.SetActivate(true);
        m_TouchDelta.RemoveExtraBinding();
        OnExitFarm?.Invoke();
    }
}
