using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampUIManager : UIManager {
    public static new CampUIManager Instance { get; private set; }
    protected override void Init()
    {
        base.Init();
        Instance = this;
        btn_Reload.SetActivate(false);

        ShowControls<UIC_PlayerStatus>().SetInGame(false);
        ShowControls<UIC_CurrencyStatus>();
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        Instance = null;
    }
    
    Action OnExitFarm;
    public UIC_FarmStatus BeginFarm(Action<bool,Vector2> _OnDragDown, Action<Vector2> _OnDrag,Action _OnExit)
    {
        m_TouchDelta.AddDragBinding(_OnDragDown, _OnDrag);
        OverrideSetting(ExitFarm);
        tf_BaseControl.localScale = Vector3.zero;
        UIC_FarmStatus target = ShowControls<UIC_FarmStatus>();
        return target;
    }
    public void ExitFarm()
    {
        tf_BaseControl.localScale=Vector3.one;
        OverrideSetting(null);
        m_TouchDelta.RemoveExtraBinding();
        OnExitFarm?.Invoke();
    }
}
