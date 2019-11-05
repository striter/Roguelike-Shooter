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

        ShowTools<UIT_CampStatus>();
        ShowTools<UIT_GamePlayerStatus>().SetInGame(false);
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        Instance = null;
    }
    
    Action OnExitFarm;
    public UIT_FarmStatus BeginFarm(Action<bool,Vector2> _OnDragDown, Action<Vector2> _OnDrag)
    {
        m_TouchDelta.AddDragBinding(_OnDragDown, _OnDrag);
        tf_Control.localScale = Vector3.zero;
        UIT_FarmStatus target = ShowTools<UIT_FarmStatus>();
        return target;
    }
    public void ExitFarm()
    {
        tf_Control.localScale=Vector3.one;
        m_TouchDelta.RemoveExtraBinding();
        OnExitFarm?.Invoke();
    }
}
